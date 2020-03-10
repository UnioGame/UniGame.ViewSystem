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
        private ViewController elementsController;
        
        private IViewFactory viewFactory;

        private List<IViewController> viewControllers = new List<IViewController>();
        
        #endregion

        public GameViewSystem(IViewResourceProvider resourceProvider, Canvas windowsCanvas, Canvas screenCanvas)
        {
            viewFactory = new ViewFactory(resourceProvider);
            
            windowsController  = new CanvasViewController(windowsCanvas,viewFactory,this).AddTo(LifeTime);
            screensController  = new CanvasViewController(screenCanvas,viewFactory,this).AddTo(LifeTime);
            elementsController = new ViewController(viewFactory,this).AddTo(LifeTime);
        }
        
        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;
        
        public void Dispose() => lifeTimeDefinition.Terminate();

        public async UniTask<T> Open<T>(IViewModel viewModel,string skinTag = "") where T : Component, IView
        {
            return await elementsController.Open<T>(viewModel,skinTag);
        }

        public async UniTask<T> OpenWindow<T>(IViewModel viewModel,string skinTag = "") where T : Component, IView
        {
            return await windowsController.Open<T>(viewModel,skinTag);
        }

        public async UniTask<T> OpenScreen<T>(IViewModel viewModel,string skinTag = "") where T : Component, IView
        {
            return await screensController.Open<T>(viewModel,skinTag);
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
        
    }
}
