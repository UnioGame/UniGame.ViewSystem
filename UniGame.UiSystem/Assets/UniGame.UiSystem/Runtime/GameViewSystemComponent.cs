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
        public ViewSystemSettings settings;
        
        public Canvas screenCanvas;

        public Canvas windowsCanvas;
        
        #endregion

        private IGameViewSystem gameViewSystem;
        
        #region view system api


        public ILifeTime LifeTime => gameViewSystem.LifeTime;

        public UniTask<T> Create<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView
        {
            return gameViewSystem.Create<T>(viewModel, skinTag); 
        }

        public UniTask<T> CreateWindow<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView
        {
            return gameViewSystem.CreateWindow<T>(viewModel, skinTag);
        }

        public UniTask<T> CreateScreen<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView
        {
            return gameViewSystem.CreateScreen<T>(viewModel, skinTag);
        }

        public void Dispose() => gameViewSystem.Dispose();
        
        #endregion

        private void Awake()
        {
            settings.Initialize();
            var factory = new ViewFactory(settings.UIResourceProvider);
            gameViewSystem = new GameViewSystem(factory,windowsCanvas,screenCanvas);
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}
