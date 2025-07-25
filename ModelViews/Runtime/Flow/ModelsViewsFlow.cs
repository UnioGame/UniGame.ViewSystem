﻿namespace UniGame.UiSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using ModelViewsMap.Runtime.Settings;
    using R3;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UniModules.UniGame.UiSystem.Runtime;
    using ViewSystem.Runtime;
     
    

    public static class ModelsViewsFlow
    {
        private static IGameViewSystem _gameViewSystem;
        private static IModelViewsSettings _modelViewsSettings;
        
        
        private static Dictionary<IViewModel,List<IViewHandle>> _viewsMap = new(256);

        
        public static void Initialize(IGameViewSystem gameViewSystem, IModelViewsSettings modelViewsSettings)
        {
            _gameViewSystem = gameViewSystem;
            _modelViewsSettings = modelViewsSettings;
        }

        public static Observable<IViewHandle> AsView(this IViewModel viewModel)
        {
            var handle = AsView(viewModel, null);
            return handle;
        }
        
        public static Observable<IViewHandle> AsView<TView>(this IViewModel viewModel)
            where TView : IView
        {
            var handle = AsView(viewModel,typeof(TView));
            return handle;
        }

        public static Observable<IViewHandle> OpenWindow(this Observable<IViewHandle> handleObservable, string skinTag = "") => 
            OpenView(handleObservable, ViewType.Window, skinTag);
        
        public static Observable<IViewHandle> OpenScreen(this Observable<IViewHandle> handleObservable, string skinTag = "") => 
            OpenView(handleObservable, ViewType.Screen, skinTag);
        
        public static Observable<IViewHandle> OpenOverlay(this Observable<IViewHandle> handleObservable, string skinTag = "") => 
            OpenView(handleObservable, ViewType.Overlay, skinTag);

        public static Observable<IViewHandle> HideView(this Observable<IViewHandle> handle)
        {
            var result = handle.
                Where(x => x.Status.CurrentValue == ViewStatus.Shown || 
                           x.Status.CurrentValue == ViewStatus.Showing).
                Do(x => x.Hide());
            return result;
        }
        
        public static Observable<IViewHandle> ShowView(this Observable<IViewHandle> handle)
        {
            var result = handle.
                Do(x => x.Show());
            return result;
        }
        
        public static Observable<IViewHandle> DestroyView(this Observable<IViewHandle> handle)
        {
            var result = handle.
                Do(x => x.Destroy());
            return result;
        }
        
        public static Observable<IViewHandle> CloseView(this Observable<IViewHandle> handle)
        {
            var result= handle.
                Do(x => x.Close());
            return result;
        }
        
        public static void CloseAll() => _gameViewSystem.CloseAll();

        #region private methods

        private static Observable<IViewHandle> OpenView(
            this Observable<IViewHandle> handleObservable, 
            ViewType layoutType,
            string skinTag = "")
        {
            var observable = handleObservable.
                Do( async x => {
                    if (x.View!= null)//TODO check layout type
                        return;
                    var view = await OpenView(x,layoutType, skinTag);
                    x.SetView(view);
                });

            return observable;
        }
        
        private static async UniTask<IView> OpenView(IViewHandle handle,ViewType layoutType, string skinTag)
        {
            IView view = null;
            var model = handle.Model;
            var viewType = handle.ViewType;
            
            switch (layoutType) {
                case ViewType.None:
                    view = await _gameViewSystem.Create(model, viewType, skinTag);
                    break;
                case ViewType.Screen:
                    view = await _gameViewSystem.OpenScreen(model, viewType, skinTag);
                    break;
                case ViewType.Window:
                    view = await _gameViewSystem.OpenWindow(model, viewType, skinTag);
                    break;
                case ViewType.Overlay:
                    view = await _gameViewSystem.OpenOverlay(model, viewType, skinTag);
                    break;
            }

            return view;

        }
        
        private static Observable<IViewHandle> AsView(this IViewModel viewModel, Type viewType)
        {
            var handler    = GetHandle(viewModel,viewModel.GetType(),viewType);
            var observable = handler;
            return observable.ToObservable();
        }
        
        private static void Remove(IViewModel viewModel)
        {
            if (_viewsMap.TryGetValue(viewModel, out var viewHandles)) {
                for (int i = 0; i < viewHandles.Count; i++) {
                    var handle = viewHandles[i];
                    handle.Dispose();
                }
                viewHandles.Despawn();
            }
            _viewsMap.Remove(viewModel);
        }
        
        private static IViewHandle GetHandle(IViewModel model,Type modelType, Type viewType = null)
        {
            //register all viewmodel view types
            if (!_viewsMap.TryGetValue(model, out var handles)) {
                
                handles = new List<IViewHandle>();
                var settingsViewTypes = _modelViewsSettings[modelType];
                foreach (var type in settingsViewTypes) {
                    var defaultHandle = new ViewHandle(model,modelType,type);
                    handles.Add(defaultHandle);
                }
                _viewsMap[model] = handles;
                model.LifeTime.AddCleanUpAction(() => Remove(model));
            }

            var handle = handles.FirstOrDefault(x => x.ViewType == viewType);
            if (handle == null && viewType == null) {
                return handles.FirstOrDefault();
            }
            
            handle = new ViewHandle(model,modelType,viewType);
            handles.Add(handle);
            
            return handle;
        }
        
        #endregion
        
    }
}
