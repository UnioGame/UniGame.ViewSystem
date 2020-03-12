namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx.Async;
    using UnityEngine;

    [Serializable]
    public class GameViewSystem : IGameViewSystem
    {
        #region private fields

        private LifeTimeDefinition lifeTimeDefinition = new LifeTimeDefinition();
        
        private IViewFactory viewFactory;

        private Dictionary<ViewType, IViewStackController> viewControllers = new Dictionary<ViewType, IViewStackController>(4);
        
        #endregion

        public GameViewSystem(
            IViewFactory viewFactory, 
            Canvas windowsCanvas, 
            Canvas screenCanvas,
            Canvas overlayCanvas)
        {
            this.viewFactory = viewFactory;

            viewControllers[ViewType.Window] = new CanvasViewController(windowsCanvas).AddTo(LifeTime); ;
            viewControllers[ViewType.Screen] = new CanvasViewController(screenCanvas).AddTo(LifeTime); ;
            viewControllers[ViewType.Overlay] = new CanvasViewController(overlayCanvas).AddTo(LifeTime); ;
        }

        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        /// <summary>
        /// terminate game view system lifetime
        /// </summary>
        public void Dispose() => lifeTimeDefinition.Terminate();

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
            var view = await viewFactory.Create<T>(skinTag, parent);

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
            if (viewControllers.TryGetValue(viewType, out var controller)) {
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
            foreach (var viewController in viewControllers.Values) {
                if(viewController.Remove(view))
                    break;
            }
            //TODO move to pool
            UnityEngine.Object.Destroy(view.gameObject);
        }
    }
}
