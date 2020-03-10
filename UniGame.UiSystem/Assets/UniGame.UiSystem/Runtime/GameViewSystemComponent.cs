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

        public UniTask<T> Open<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView => gameViewSystem.Open<T>(viewModel, skinTag);

        public UniTask<T> OpenWindow<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView => gameViewSystem.OpenWindow<T>(viewModel, skinTag);

        public UniTask<T> OpenScreen<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView => gameViewSystem.OpenScreen<T>(viewModel, skinTag);

        public bool CloseWindow<T>()
            where T : Component, IView => gameViewSystem.CloseWindow<T>();

        public bool CloseScreen<T>()
            where T : Component, IView => gameViewSystem.CloseScreen<T>();
        
        public void Dispose() => gameViewSystem.Dispose();
        
        #endregion

        private void Awake()
        {
            settings.Initialize();
            gameViewSystem = new GameViewSystem(settings.UIResourceProvider,windowsCanvas,screenCanvas);
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}
