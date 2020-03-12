using UnityEngine;

namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using Abstracts;
    using Settings;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniRx.Async;

    public class GameViewSystemComponent : MonoBehaviour, IGameViewSystem
    {
        
        #region inspector data
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Required]
        [Sirenix.OdinInspector.InlineEditor]
#endif
        public UiSystemSettings settings;
        
        public Canvas screenCanvas;

        public Canvas windowsCanvas;

        public Canvas overlayCanvas;
        
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

        public void Dispose() => gameViewSystem.Dispose();
        
        #endregion

        private void Awake()
        {
            settings.Initialize();
            var factory = new ViewFactory(settings.UIResourceProvider);
            gameViewSystem = new GameViewSystem(factory,windowsCanvas,screenCanvas, overlayCanvas);
        }

        private void OnDestroy() => Dispose();
    }
}
