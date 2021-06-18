using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Abstract;
using UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Extensions;
using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Settings;
    using UniModules.UniGame.UiSystem.Runtime;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime.Abstract;

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

        #region IViewModelProvider api

        public bool IsValid(Type modelType) => ViewSystem.IsValid(modelType);

        public async UniTask<IViewModel> Create(IContext context,Type modelType) => await ViewSystem.Create(context,modelType);
        
        #endregion
        
        #region view system api

        public IViewModelTypeMap ModelTypeMap => ViewSystem.ModelTypeMap;
        
        public IObservable<IView> ViewCreated => ViewSystem.ViewCreated;
        
        public IGameViewSystem    ViewSystem  => gameViewSystem ?? (gameViewSystem = Create());

        public ILifeTime LifeTime => ViewSystem.LifeTime;

        public IEnumerable<IViewLayout> Controllers => ViewSystem.Controllers;
        
        public IReadOnlyViewLayout this[ViewType type] => ViewSystem[type];

        public IObservable<TView> ObserveView<TView>() where  TView :class, IView => ViewSystem.ObserveView<TView>();

        public UniTask<IView> Create(IViewModel viewModel, Type viewType, string skinTag = "", Transform parent = null, string viewName = null, bool stayWorld = false) =>
            ViewSystem.Create(viewModel, viewType, skinTag, parent, viewName,stayWorld);

        public UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenWindow(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> OpenScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenScreen(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> OpenOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.OpenOverlay(viewModel, viewType, skinTag, viewName);
        
        public UniTask<IView> CreateWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.CreateWindow(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> CreateScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.CreateScreen(viewModel, viewType, skinTag, viewName);

        public UniTask<IView> CreateOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null) => ViewSystem.CreateOverlay(viewModel, viewType, skinTag, viewName);

        public IViewLayout GetLayout(ViewType type) => ViewSystem.GetLayout(type);

        public T Get<T>() where T : class, IView
        {
            return ViewSystem.Get<T>();
        }

        public void CloseAll()
        {
            ViewSystem.CloseAll();
        }

        public void Dispose() => gameViewSystem?.Dispose();
        
        #endregion

        private void OnDestroy() => Dispose();

        private IGameViewSystem Create()
        {
            settings.Initialize();

            var factory  = new ViewFactory(new AsyncLazy(() => settings.WaitForInitialize()),settings.ResourceProvider);
            var stackMap = new Dictionary<ViewType, IViewLayout>(4);
            foreach (var item in layoutMap) {
                stackMap[item.Key] = item.Value;
            }
            
            var viewLayoutContainer = new ViewStackLayoutsContainer(stackMap);
            var sceneFlowController = settings.FlowController;

            var gameSystem = new GameViewSystem(factory, viewLayoutContainer, sceneFlowController,settings.viewsModelProviderSettings,settings.ViewModelTypeMap);

            gameSystem.TryMakeActive();
            
            return gameSystem;
        }

    }
}
