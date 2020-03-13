using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime.Settings;
    using UniRx.Async;

    public class GameViewSystemAsset : MonoBehaviour, IGameViewSystem
    {
        
        #region inspector data
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Required]
        [Sirenix.OdinInspector.InlineEditor]
#endif
        public ViewSystemSettings settings;
        
        [Space]
        public ViewLayoutMap layoutMap = new ViewLayoutMap();

        #endregion

        private IGameViewSystem gameViewSystem;
        
        #region view system api


        public ILifeTime LifeTime => gameViewSystem.LifeTime;

        public UniTask<T> Create<T>(IViewModel viewModel, string skinTag = "", Transform parent = null)
            where T : Component, IView
        {
            return gameViewSystem.Create<T>(viewModel, skinTag); 
        }

        public UniTask<T> OpenWindow<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView
        {
            return gameViewSystem.OpenWindow<T>(viewModel, skinTag);
        }

        public UniTask<T> OpenScreen<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView
        {
            return gameViewSystem.OpenScreen<T>(viewModel, skinTag);
        }

        public UniTask<T> OpenOverlay<T>(IViewModel viewModel, string skinTag = "") 
            where T : Component, IView
        {
            return gameViewSystem.OpenOverlay<T>(viewModel, skinTag);
        }

        public void CloseAll()
        {
            gameViewSystem.CloseAll();
        }

        public void Dispose() => gameViewSystem.Dispose();
        
        #endregion

        private void Awake()
        {
            settings.Initialize();
            
            var factory = new ViewFactory(settings.UIResourceProvider);
            var stackMap = new Dictionary<ViewType, IViewStackController>(4);
            
            foreach (var item in layoutMap) {
                stackMap[item.Key] = item.Value;
            }
            
            gameViewSystem = new GameViewSystem(factory,stackMap);
        }

        private void OnDestroy() => Dispose();
    }
}
