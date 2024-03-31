using UnityEngine;
using YooAsset;

namespace AOT
{
    public class Initialize : MonoBehaviour
    {
        /// <summary>
        /// 资源系统运行模式
        /// </summary>
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        void Awake()
        {
            Debug.Log($"资源系统运行模式：{PlayMode}");
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            DontDestroyOnLoad(this.gameObject);
        }

        private async void Start()
        {
            // 初始化资源系统
            YooAssets.Initialize();
            // 开始补丁更新流程
            PatchOperation operation = new PatchOperation("DefaultPackage",
                EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(), PlayMode);
            YooAssets.StartOperation(operation);
            await operation.Task;

            // 设置默认的资源包
            var gamePackage = YooAssets.GetPackage("DefaultPackage");
            YooAssets.SetDefaultPackage(gamePackage);
            gameObject.AddComponent<LoadDll>();
        }
    }
}