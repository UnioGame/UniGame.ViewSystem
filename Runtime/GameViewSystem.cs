using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;
using UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Abstract;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]

namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.UiSystem.Runtime;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class GameViewSystem : IGameViewSystem
    {
        #region private fields

        private LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();

        private readonly IViewFactory _viewFactory;
        private readonly IViewLayoutContainer _viewLayouts;
        private readonly IViewFlowController _flowController;
        private readonly IViewModelProvider _viewModelProvider;
        private readonly IViewModelTypeMap _modelTypeMap;
        private readonly Subject<IView> _viewCreatedSubject;

        #endregion

        public GameViewSystem(
            IViewFactory viewFactory,
            IViewLayoutContainer viewLayouts,
            IViewFlowController flowController,
            IViewModelProvider viewModelProvider,
            IViewModelTypeMap modelTypeMap)
        {
            _viewCreatedSubject = new Subject<IView>().AddTo(_lifeTimeDefinition);

            _viewFactory = viewFactory;
            _viewLayouts = viewLayouts;
            _flowController = flowController;
            _viewModelProvider = viewModelProvider;
            _modelTypeMap = modelTypeMap;

            _flowController.Activate(_viewLayouts);
        }
        
        public IViewModelTypeMap ModelTypeMap => _modelTypeMap;

        public ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        /// <summary>
        /// reactive views stream
        /// </summary>
        public IObservable<IView> ViewCreated => _viewCreatedSubject;

        #region public methods

        /// <summary>
        /// terminate game view system lifetime
        /// </summary>
        public void Dispose() => _lifeTimeDefinition.Terminate();

        #region IViewModelProvider api

        public bool IsValid(Type modelType) => _viewModelProvider.IsValid(modelType);

        public async UniTask<IViewModel> Create(IContext context, Type modelType)
        {
            if (_viewModelProvider == null)
                return null;
            return await _viewModelProvider.Create(context,modelType);
        }
        
        #endregion
        
        #region ui system api

        public IObservable<TView> ObserveView<TView>()
            where TView : class, IView
        {
            return ViewCreated.OfType<IView, TView>();
        }

        public async UniTask<IView> Create(IViewModel viewModel, Type viewType, string skinTag = "",
            Transform parent = null, string viewName = null, bool stayWorld = false)
        {
            return await CreateView(viewModel, viewType, skinTag, parent, viewName, stayWorld);
        }

        public async UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, string skinTag = "",
            string viewName = null)
        {
            return await OpenView<ViewBase>(viewModel, viewType, ViewType.Window, skinTag, viewName);
        }

        public async UniTask<IView> OpenScreen(IViewModel viewModel, Type viewType, string skinTag = "",
            string viewName = null)
        {
            return await OpenView<ViewBase>(viewModel, viewType, ViewType.Screen, skinTag, viewName);
        }

        public async UniTask<IView> OpenOverlay(IViewModel viewModel, Type viewType, string skinTag = "",
            string viewName = null)
        {
            return await OpenView<ViewBase>(viewModel, viewType, ViewType.Overlay, skinTag, viewName);
        }

        public T Get<T>() where T : class, IView
        {
            foreach (var controller in _viewLayouts.Controllers)
            {
                var v = controller.Get<T>();
                if (v != null)
                    return v;
            }

            return null;
        }

        #endregion

        #region layout container api

        public IReadOnlyViewLayout this[ViewType type] => _viewLayouts[type];

        public IEnumerable<IViewLayout> Controllers => _viewLayouts.Controllers;

        public IViewLayout GetLayout(ViewType type) => _viewLayouts.GetLayout(type);

        #endregion

        public void CloseAll()
        {
            _viewLayouts.GetLayout(ViewType.Screen)?.CloseAll();
            _viewLayouts.GetLayout(ViewType.Window)?.CloseAll();
        }

        /// <summary>
        /// create new view element
        /// </summary>
        /// <param name="viewModel">target element model data</param>
        /// <param name="viewType">view type filter</param>
        /// <param name="skinTag">target element skin</param>
        /// <param name="parent">view parent</param>
        /// <param name="viewName"></param>
        /// <param name="stayWorld"></param>
        /// <returns>created view element</returns>
        public async UniTask<IView> CreateView(
            IViewModel viewModel,
            Type viewType,
            string skinTag = "",
            Transform parent = null,
            string viewName = null,
            bool stayWorld = false)
        {
            var view = (await _viewFactory.Create(viewType, skinTag, parent, viewName, stayWorld));

            await InitializeView(view, viewModel);

            return view;
        }

        /// <summary>
        /// create new view element
        /// </summary>
        /// <param name="viewModel">target element model data</param>
        /// <param name="viewType">view type filter</param>
        /// <param name="skinTag">target element skin</param>
        /// <param name="parent">view parent</param>
        /// <returns>created view element</returns>
        public async UniTask<T> CreateView<T>(
            IViewModel viewModel,
            string skinTag = "",
            Transform parent = null,
            bool stayWorld = false)
            where T : class, IView
        {
            var view = await CreateView(viewModel, typeof(T), skinTag, parent, String.Empty, stayWorld) as T;
            return view;
        }

        #endregion


        #region private methods

        /// <summary>
        /// create view on target controller
        /// </summary>
        private async UniTask<T> OpenView<T>(
            IViewModel viewModel,
            Type viewType,
            ViewType layoutType,
            string skinTag = "",
            string viewName = null)
            where T : class, IView
        {
            var layout = _viewLayouts.GetLayout(layoutType);
            var parent = layout?.Layout;

            var view = await CreateView(viewModel, viewType, skinTag, parent,viewName);

            layout?.Push(view);

            return view as T;
        }

        /// <summary>
        /// Initialize View with model data
        /// </summary>
        private async UniTask<T> InitializeView<T>(T view, IViewModel viewModel)
            where T : IView
        {
            if (view is ILayoutFactoryView factoryView)
                factoryView.BindLayout(this);

            await view.Initialize(viewModel);

            //destroy view when lifetime terminated
            var viewLifeTime = view.LifeTime;
            viewLifeTime.AddCleanUpAction(() => Destroy(view));

            //fire view data
            _viewCreatedSubject.OnNext(view);

            return view;
        }

        private void Destroy(IView view)
        {
            view.Destroy();

            //TODO move to pool
            var asset = view as Component;

            if (asset != null)
            {
                var target = asset.gameObject;
                UnityEngine.Object.Destroy(target);
            }
        }

        #endregion
    }
}