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

        private CanvasViewController windowsController;
        private CanvasViewController screensController;
        private ViewStackController elementsController;

        private IViewFactory viewFactory;

        private List<IViewStackController> viewControllers = new List<IViewStackController>();

        #endregion

        public GameViewSystem(IViewFactory viewFactory, Canvas windowsCanvas, Canvas screenCanvas)
        {
            this.viewFactory = viewFactory;

            windowsController = new CanvasViewController(windowsCanvas).AddTo(LifeTime);
            screensController = new CanvasViewController(screenCanvas).AddTo(LifeTime);

            viewControllers.Add(windowsController);
            viewControllers.Add(screensController);
            viewControllers.Add(elementsController);
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
            var view = await CreateView<T>(viewModel, skinTag, windowsController.Canvas.transform);
            //add created view to target controller
            screensController.Add(view);

            return view;
        }

        public async UniTask<T> OpenScreen<T>(IViewModel viewModel, string skinTag = "") where T : Component, IView
        {
            var view = await CreateView<T>(viewModel, skinTag, screensController.Canvas.transform);

            //add created view to target controller
            screensController.Add(view);

            return view;
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
            foreach (var viewController in viewControllers)
            {
                if (viewController.Remove(view))
                    break;
            }
            //TODO move to pool
            UnityEngine.Object.Destroy(view.gameObject);
        }
    }
}
