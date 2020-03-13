using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System.Collections.Generic;
    using Abstracts;
    using Settings;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniGame.UiSystem.Runtime;
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

        public IEnumerable<IViewLayoutController> Controllers => gameViewSystem.Controllers;
        
        public IReadOnlyViewLayoutController this[ViewType type] => gameViewSystem[type];

        public IViewLayoutController GetViewController(ViewType type) => gameViewSystem.GetViewController(type);

        public void Dispose() => gameViewSystem.Dispose();
        
        #endregion

        private void Awake()
        {
            settings.Initialize();
            
            var factory = new ViewFactory(settings.UIResourceProvider);
            var stackMap = new Dictionary<ViewType, IViewLayoutController>(4);
            foreach (var item in layoutMap) {
                stackMap[item.Key] = item.Value;
            }
            
            var viewLayoutContainer = new ViewStackLayoutsContainer(stackMap);
            var sceneFlowController = new ViewSceneFlowController(viewLayoutContainer);

            gameViewSystem = new GameViewSystem(factory, viewLayoutContainer, sceneFlowController);
        }

        private void OnDestroy() => Dispose();

    }
}
