using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

//
// public class AssetBundleCollectorPackageDrawer : OdinValueDrawer<AssetBundleCollectorPackage>
// {
//     protected override void DrawPropertyLayout(GUIContent label)
//     {
//         var parent = ValueEntry.Property;
//         Debug.Log(parent.IsTreeRoot);
//         if (parent.IsTreeRoot)
//         {
//             return;
//         }
//
//         base.CallNextDrawer(label);
//     }
// }

public static class AssetBundleCollectorExtension
{
    public static string FullName(this AssetBundleCollectorPackage package)
    {
        return string.IsNullOrEmpty(package.PackageDesc)
            ? package.PackageName
            : $"{package.PackageName} ({package.PackageDesc})";
    }

    public static string FullName(this AssetBundleCollectorGroup group)
    {
        return string.IsNullOrEmpty(group.GroupDesc)
            ? group.GroupName
            : $"{group.GroupName} ({group.GroupDesc})";
    }
}

public class OdinTreeYooAssetsWindow : OdinMenuEditorWindow
{
    [MenuItem("Tools/Odin Tree YooAssets Window")]
    public static void Open()
    {
        GetWindow<OdinTreeYooAssetsWindow>();
    }

    private AssetBundleCollectorSetting _setting;

    private List<string> _collectorTypeList;
    private List<RuleDisplayName> _activeRuleList;
    private List<RuleDisplayName> _addressRuleList;
    private List<RuleDisplayName> _packRuleList;
    private List<RuleDisplayName> _filterRuleList;

    protected override void OnEnable()
    {
        _setting = SettingLoader.LoadSettingData<AssetBundleCollectorSetting>();
        _collectorTypeList = new List<string>()
        {
            $"{nameof(ECollectorType.MainAssetCollector)}",
            $"{nameof(ECollectorType.StaticAssetCollector)}",
            $"{nameof(ECollectorType.DependAssetCollector)}"
        };
        _activeRuleList = AssetBundleCollectorSettingData.GetActiveRuleNames();
        _addressRuleList = AssetBundleCollectorSettingData.GetAddressRuleNames();
        _packRuleList = AssetBundleCollectorSettingData.GetPackRuleNames();
        _filterRuleList = AssetBundleCollectorSettingData.GetFilterRuleNames();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(supportsMultiSelect: true);

        foreach (var package in _setting.Packages)
        {
            var packageName = package.FullName();
            tree.Add(packageName, package);
            foreach (var group in package.Groups)
            {
                var groupName = group.FullName();
                tree.Add($"{packageName}/{groupName}", group);
            }
        }

        return tree;
    }

    protected override void DrawEditors()
    {
        if (MenuTree.Selection != null)
        {
            foreach (var select in MenuTree.Selection)
            {
                switch (select.Value)
                {
                    case AssetBundleCollectorPackage package:
                    {
                        package.PackageName =
                            SirenixEditorFields.TextField(nameof(package.PackageName), package.PackageName);
                        package.PackageDesc =
                            SirenixEditorFields.TextField(nameof(package.PackageDesc), package.PackageDesc);
                        select.Name = package.FullName();
                    }
                        break;
                    case AssetBundleCollectorGroup group:
                    {
                        group.GroupName =
                            SirenixEditorFields.TextField(nameof(group.GroupName), group.GroupName);
                        group.GroupDesc =
                            SirenixEditorFields.TextField(nameof(group.GroupDesc), group.GroupDesc);
                        SirenixEditorFields.Dropdown(nameof(group.ActiveRuleName),
                            _activeRuleList.FindIndex(x => x.ClassName == group.ActiveRuleName)
                            , _activeRuleList.Select(x => x.ClassName).ToArray());
                        group.AssetTags =
                            SirenixEditorFields.TextField(nameof(group.AssetTags), group.AssetTags);
                        select.Name = group.FullName();
                    }
                        break;
                    default:
                        base.DrawEditors();
                        break;
                }
            }
        }
    }
}