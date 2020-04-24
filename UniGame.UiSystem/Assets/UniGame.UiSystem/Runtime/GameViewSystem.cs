namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.UiSystem.Runtime;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    public class GameViewSystem : IGameViewSystem
    {
        #region private fields

        private LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();

        private readonly IViewFactory _viewFactory;
        private readonly IViewLayoutContainer _viewLayouts;
        private readonly IViewFlowController _flowController;

        #endregion

        public GameViewSystem(
            IViewFactory viewFactory,
            IViewLayoutContainer viewLayouts,
            IViewFlowController flowController)
        {
            _viewFactory = viewFactory;
            _viewLayouts = viewLayouts;
            _flowController = flowController;
            
            _flowController.Activate(_viewLayouts);
        }

        public ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        /// <summary>
        /// terminate game view system lifetime
        /// </summary>
        public void Dispose() => _lifeTimeDefinition.Terminate();

        #region ui system api

        public async UniTask<T> Create<T>(IViewModel viewModel, string skinTag = "", Transform parent = null) where T :class, IView
        {
            return await CreateView<T>(viewModel,typeof(T), skinTag, parent);
        }

        public async UniTask<T> OpenWindow<T>(IViewModel viewModel, string skinTag = "")where T :class, IView
        {
            return await OpenView<T>(viewModel,typeof(T), ViewType.Window, skinTag);
        }

        public async UniTask<T> OpenScreen<T>(IViewModel viewModel, string skinTag = "") where T :class, IView
        {
            return await OpenView<T>(viewModel,typeof(T), ViewType.Screen, skinTag);
        }

        public async UniTask<T> OpenOverlay<T>(IViewModel viewModel, string skinTag = "") where T :class, IView
        {
            return await OpenView<T>(viewModel,typeof(T), ViewType.Overlay, skinTag);
        }

        //Direct type methods
        
        public async UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, string skinTag = "")
        {
            return await OpenView<ViewBase>(viewModel,viewType, ViewType.Window, skinTag);
        }

        public async UniTask<IView> OpenScreen(IViewModel viewModel,Type viewType, string skinTag = "")
        {
            return await OpenView<ViewBase>(viewModel,viewType, ViewType.Screen, skinTag);
        }

        public async UniTask<IView> OpenOverlay(IViewModel viewModel,Type viewType, string skinTag = "") 
        {
            return await OpenView<ViewBase>(viewModel,viewType, ViewType.Overlay, skinTag);
        }
        
        public T Get<T>() where T : Component, IView
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
        /// <returns>created view element</returns>
        public async UniTask<T> CreateView<T>(
            IViewModel viewModel,
            Type viewType,
            string skinTag = "",
            Transform parent = null)
            where T :class, IView
        {
            var view = (await _viewFactory.Create(viewType,skinTag, parent)) as T;

            InitializeView(view, viewModel);

            return view;

        }

        #region private methods

        /// <summary>
        /// create view on target controller
        /// </summary>
        private async UniTask<T> OpenView<T>(
            IViewModel viewModel,
            Type viewType,
            ViewType layoutType,
            string skinTag = "")
            where T : class, IView
        {
            var layout = _viewLayouts.GetLayout(layoutType);
            var parent = layout?.Layout;

            var view = await CreateView<T>(viewModel,viewType, skinTag, parent);

            layout?.Push(view);

            return view;
        }

        /// <summary>
        /// Initialize View with model data
        /// </summary>
        private T InitializeView<T>(T view, IViewModel viewModel)
            where T : IView
        {

            view.Initialize(viewModel, this);
            //destroy view when lifetime  terminated
            var viewLifeTime = view.LifeTime;
            viewLifeTime.AddCleanUpAction(() => Destroy(view));

            return view;
        }

        private void Destroy(IView view)
        {
            view.Destroy();
            
            //TODO move to pool
            var asset = view as Component;
            var target = asset?.gameObject;
            
            if(target != null)
                UnityEngine.Object.Destroy(target);
        }


        #endregion
 }
}
