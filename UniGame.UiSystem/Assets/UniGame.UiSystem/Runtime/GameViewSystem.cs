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
        private readonly IViewSceneTransitionController _sceneTransitionController;

        #endregion

        public GameViewSystem(
            IViewFactory viewFactory,
            IViewLayoutContainer viewLayouts,
            IViewSceneTransitionController sceneTransitionController)
        {
            _viewFactory = viewFactory;
            _viewLayouts = viewLayouts;
            _sceneTransitionController = sceneTransitionController;

            BindSceneActions();

        }

        public ILifeTime LifeTime => _lifeTimeDefinition.LifeTime;

        /// <summary>
        /// terminate game view system lifetime
        /// </summary>
        public void Dispose() => _lifeTimeDefinition.Terminate();

        #region ui system api

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

        public IViewLayout GetViewController(ViewType type) => _viewLayouts.GetViewController(type);


        #endregion

        public void CloseAll()
        {
            _viewLayouts[ViewType.Screen].CloseAll();
            _viewLayouts[ViewType.Window].CloseAll();
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

        #region private methods

        /// <summary>
        /// create view on target controller
        /// </summary>
        private async UniTask<T> OpenView<T>(
            IViewModel viewModel,
            ViewType viewType,
            string skinTag = "")
            where T : Component, IView
        {
            var layout = _viewLayouts.GetViewController(viewType);
            var parent = layout?.Layout;

            var view = await CreateView<T>(viewModel, skinTag, parent);

            layout?.Push(view);

            view.Show();
            
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
            foreach (var viewController in _viewLayouts.Controllers) {
                if(viewController.Close(view))
                    break;

            }
            
            //TODO move to pool
            UnityEngine.Object.Destroy(view.gameObject);
        }


        private void BindSceneActions()
        {
            Observable.FromEvent(
                    x => SceneManager.activeSceneChanged += _sceneTransitionController.OnSceneActivate,
                    x => SceneManager.activeSceneChanged -= _sceneTransitionController.OnSceneActivate).
                Subscribe().
                AddTo(LifeTime);

            Observable.FromEvent(
                    x => SceneManager.sceneLoaded += _sceneTransitionController.OnSceneLoaded,
                    x => SceneManager.sceneLoaded -= _sceneTransitionController.OnSceneLoaded).
                Subscribe().
                AddTo(LifeTime);

            Observable.FromEvent(
                    x => SceneManager.sceneUnloaded += _sceneTransitionController.OnSceneUnloaded,
                    x => SceneManager.sceneUnloaded -= _sceneTransitionController.OnSceneUnloaded).
                Subscribe().
                AddTo(LifeTime);
        }

        #endregion
 }
}
