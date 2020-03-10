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
        
        // контролеры отдельных канвасов имеют слишком много не нужного, предлагаю разделить
        // логику factory и логику управления стэком
        // чтобы elements controller стал фабрикой через которую все элементы создаются,
        // а в контроллеры скринов и окон после создания дергаются add или register и вся
        // логика show/hide/suspend etc. управляется этими контроллерами
        // это упростит структуру классов и уберёт наследование между контроллерами
        private CanvasViewController windowsController;
        private CanvasViewController screensController;
        private ViewController elementsController;
        
        private IViewFactory viewFactory;

        private List<IViewStackController> viewControllers = new List<IViewStackController>();
        
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

        // open по смыслу - create
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
        
        
        /// <summary>
        /// Open new view element
        /// </summary>
        /// <param name="viewModel">target element model data</param>
        /// <param name="skinTag">target element skin</param>
        /// <returns>created view element</returns>
        public async UniTask<T> CreateView<T>(IViewModel viewModel,string skinTag = "") 
            where T : Component, IView
        {
            
            var view = await viewFactory.Create<T>(skinTag);

            //initialize view with model data
            InitializeView(view, viewModel);

            //update view properties
            OnViewOpen(view);
            
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

            //handle all view visibility changes
            view.IsActive.
                Subscribe(x => OnVisibilityChanged(view,x)).
                AddTo(viewLifeTime);
            
            return view;
        }

        private void OnVisibilityChanged(IView view,bool visibility)
        {
            //todo
        }
        
    }
}
