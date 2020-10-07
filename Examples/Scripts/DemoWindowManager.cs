using UnityEngine;

namespace UniModules.UniGame.UiSystem.Examples.BaseUiManager
{
    using AddressableTools.Runtime.Attributes;
    using AddressableTools.Runtime.Extensions;
    using global::UniGame.UiSystem.Examples.BaseUiManager;
    using global::UniGame.UiSystem.Runtime;
    using UniCore.Runtime.DataFlow;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UnityEngine.AddressableAssets;

    public enum DemoUiType
    {
        Element,
        Screen,
        Window,
    }
    
    public class DemoWindowManager : MonoBehaviour
    {
        public GameViewSystemAsset uiViewManager;

        [ShowAssetReference]
        public AssetReference nextScene;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void ShowDemoViewAsScreen(DemoUiType type,string tag = "")
        {
            switch (type) {
                case DemoUiType.Element:
                    uiViewManager.Create<DemoWindowView>(new ViewModelBase(),tag);
                    break;
                case DemoUiType.Screen:
                    uiViewManager.OpenScreen<DemoScreenView>(new ViewModelBase(),tag);
                    break;
                case DemoUiType.Window:
                    uiViewManager.OpenWindow<DemoWindowView>(new ViewModelBase(),tag);
                    break;
            }
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void ShowWindow(string skin = "")
        {
            ShowDemoViewAsScreen(DemoUiType.Window, skin);
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void ShowScreen(string skin = "")
        {
            ShowDemoViewAsScreen(DemoUiType.Screen, skin);
        }
        
        private LifeTimeDefinition lifeTimeDefinition = new LifeTimeDefinition();

        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public async void ReloadScene()
        {
            await nextScene.LoadSceneTaskAsync(lifeTimeDefinition.LifeTime);
        }

        private void OnDisable()
        {
            lifeTimeDefinition.Terminate();
        }
    }
}
