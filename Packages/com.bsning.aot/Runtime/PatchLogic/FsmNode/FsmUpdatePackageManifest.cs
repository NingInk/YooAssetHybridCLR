using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;

/// <summary>
/// 更新资源清单
/// </summary>
public class FsmUpdatePackageManifest : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }

    void IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("更新资源清单！");
        UpdateManifest();
    }

    void IStateNode.OnUpdate()
    {
    }

    void IStateNode.OnExit()
    {
    }

    async void UpdateManifest()
    {
        await Task.Delay(TimeSpan.FromSeconds(0.5f));

        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var packageVersion = (string)_machine.GetBlackboardValue("PackageVersion");
        var package = YooAssets.GetPackage(packageName);
        bool savePackageVersion = true;
        var operation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion);
        await operation.Task;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            PatchEventDefine.PatchManifestUpdateFailed.SendEventMessage();
            return;
        }
        else
        {
            _machine.ChangeState<FsmCreatePackageDownloader>();
        }
    }
}