namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using Abstracts;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;
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

        public GameViewSystem(IViewResourceProvider resourceProvider, Canvas windowsCanvas, Canvas screenCanvas)
        {
            viewFactory = new ViewFactory(resourceProvider);
            
            windowsController  = new CanvasViewController(windowsCanvas,viewFactory,this).AddTo(LifeTime);
            screensController  = new CanvasViewController(screenCanvas,viewFactory,this).AddTo(LifeTime);
            elementsController = new ViewStackController(viewFactory,this).AddTo(LifeTime);
            
            viewControllers.Add(windowsController);
            viewControllers.Add(screensController);
            viewControllers.Add(elementsController);
            
        }
        
        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;
        
        public void Dispose() => lifeTimeDefinition.Terminate();

        // open по смыслу - create
        public async UniTask<T> Open<T>(IViewModel viewModel,string skinTag = "") where T : Component, IView
        {
            return await CreateView<T>(elementsController,viewModel,skinTag);
        }

        public async UniTask<T> OpenWindow<T>(IViewModel viewModel,string skinTag = "") where T : Component, IView
        {
            return await CreateView<T>(windowsController,viewModel,skinTag);
        }

        public async UniTask<T> OpenScreen<T>(IViewModel viewModel,string skinTag = "") where T : Component, IView
        {
            return await CreateView<T>(screensController,viewModel,skinTag);
        }

        public bool CloseWindow<T>() where T : Component, IView
        {
            return windowsController.Close<T>();
        }

        public bool CloseScreen<T>() where T : Component, IView
        {
            return screensController.Close<T>();
        }


        private void Close<TView>(TView view) where TView : Component, IView
        {
            foreach (var viewController in viewControllers) {
                if(viewController.Close(view))
                    break;
            }
        }


        /// <summary>
        /// Open new view element
        /// </summary>
        /// <param name="controller">layout controller</param>
        /// <param name="viewModel">target element model data</param>
        /// <param name="skinTag">target element skin</param>
        /// <returns>created view element</returns>
        public async UniTask<T> CreateView<T>(
            IViewStackController controller,
            IViewModel viewModel,
            string skinTag = "") 
            where T : Component, IView
        {
            
            var view = await viewFactory.Create<T>(skinTag);

            //initialize view with model data
            InitializeView(view, viewModel);

            //update view properties
            controller.Add(view);
            
            return view;

        }
        
                
        private T InitializeView<T>(T view, IViewModel viewModel)
            where T : Component, IView
        {
            // Комментарии от кэпа по всему классу - не нужны

            //initialize view with model data
            view.Initialize(viewModel,this);
            
            //bind disposable to View lifeTime
            var viewLifeTime = view.LifeTime;
            
            //close view 
            viewLifeTime.AddCleanUpAction(() => Close(view));

            return view;
        }

    }
}
