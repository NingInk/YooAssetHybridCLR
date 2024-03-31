using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using HybridCLR;
using UnityEngine;
using YooAsset;

namespace AOT
{
    public class LoadDll : MonoBehaviour
    {
        async void Start()
        {
            // 先补充元数据
            await LoadMetadataForAOTAssemblies();
            // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
            var handle = YooAssets.LoadAssetAsync("Assembly-CSharp.dll");
            await handle.Task;
            Assembly.Load(((TextAsset)handle.AssetObject).bytes);
#endif
            YooAssets.LoadSceneAsync("Entry");
        }

        private async UniTask LoadMetadataForAOTAssemblies()
        {
            List<string> aotDllList = new List<string>
            {
                "mscorlib.dll",
                "System.dll",
                "System.Core.dll", // 如果使用了Linq，需要这个
                // "Newtonsoft.Json.dll", 
                // "protobuf-net.dll",
            };

            foreach (var aotDllName in aotDllList)
            {
                var handle = YooAssets.LoadAssetAsync(aotDllName);
                await handle.Task;
                // int err =
                HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(((TextAsset)handle.AssetObject).bytes,
                    HomologousImageMode.SuperSet);
                Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{HomologousImageMode.SuperSet}");
            }
        }
    }
}