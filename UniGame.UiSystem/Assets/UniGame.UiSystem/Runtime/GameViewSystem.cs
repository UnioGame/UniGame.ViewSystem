namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using global::UniGame.UiSystem.Runtime.Abstracts;
    using UniGame.UiSystem.Runtime.Abstracts;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniRx.Async;
    using UnityEngine;

    [Serializable]
    public class GameViewSystem : IGameViewSystem
    {
        #region private fields

        private LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();
        
        private IViewFactory _viewFactory;

        private IDictionary<ViewType, IViewStackController> _viewControllers;
        
        #endregion

        public GameViewSystem(
            IViewFactory viewFactory, 
            IDictionary<ViewType,IViewStackController> layoutMap)
        {
            this._viewFactory = viewFactory;
            this._viewControllers = layoutMap;
        }

        public ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        /// <summary>
        /// terminate game view system lifetime
        /// </summary>
        public void Dispose() => _lifeTimeDefinition.Terminate();

        public async UniTask<T> Create<T>(IViewModel viewModel, string skinTag = "", Transform parent = null) where T : Component, IView
        {
            return await CreateView<T>(viewModel, skinTag, parent);
        }

        public async UniTask<T> OpenWindow<T>(IViewModel viewModel, string skinTag = "") where T : Component, IView
        {
            return await OpenView<T>(viewModel, ViewType.Window, skinTag);
        }

        public async UniTask<T> OpenScreen<T>(IViewModel viewModel, string skinTag = "") where T : Component, IView
        {
            return await OpenView<T>(viewModel, ViewType.Screen, skinTag);
        }

        public async UniTask<T> OpenOverlay<T>(IViewModel viewModel, string skinTag = "") where T : Component, IView
        {
            return await OpenView<T>(viewModel, ViewType.Overlay, skinTag);
        }


        /// <summary>
        /// create new view element
        /// </summary>
        /// <param name="viewModel">target element model data</param>
        /// <param name="skinTag">target element skin</param>
        /// <returns>created view element</returns>
        public async UniTask<T> CreateView<T>(
            IViewModel viewModel,
            string skinTag = "",
            Transform parent = null)
            where T : Component, IView
        {
            var view = await _viewFactory.Create<T>(skinTag, parent);

            InitializeView(view, viewModel);

            return view;

        }

        /// <summary>
        /// create view on target controller
        /// </summary>
        private async UniTask<T> OpenView<T>(
            IViewModel viewModel,
            ViewType viewType,
            string skinTag = "")
            where T : Component, IView
        {
            Transform parent = null;
            if (_viewControllers.TryGetValue(viewType, out var controller)) {
                parent = controller.Layout;
            }

            var view = await CreateView<T>(viewModel, skinTag, parent);

            controller?.Add(view);

            return view;
        }

            /// <summary>
            /// Initialize View with model data
            /// </summary>
            private T InitializeView<T>(T view, IViewModel viewModel)
            where T : Component, IView
        {

            view.Initialize(viewModel, this);
            //destroy view when lifetime  terminated
            var viewLifeTime = view.LifeTime;
            viewLifeTime.AddCleanUpAction(() => Destroy(view));

            return view;
        }

        private void Destroy<TView>(TView view) where TView : Component, IView
        {
            foreach (var viewController in _viewControllers.Values) {
                if(viewController.Remove(view))
                    break;
            }
            //TODO move to pool
            UnityEngine.Object.Destroy(view.gameObject);
        }
    }
}
