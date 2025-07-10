namespace UniGame.ViewSystem.Runtime.Utils
{
    using System;
    using global::UniGame.ViewSystem.Runtime;
    using Cysharp.Threading.Tasks;
    using R3;
    using UniModules.UniGame.UiSystem.Runtime;
    using UnityEngine;

    public static class ViewExtensions
    {
        public static async UniTask<TView> Create<TView>(this IViewProvider source, IViewModel viewModel, Transform parent) 
            where TView : Component, IView
        {
            var view = await source.CreateView<TView>(viewModel);
            view.transform.SetParent(parent, false);

            return view;
        }

        public static async UniTask<TView> GetOrShow<TView>(this IViewsLayout viewSystem, ViewType type,
            Func<IViewModel> modelFactory,
            string skinTag = "",
            string viewName = null)
            where TView : class, IView
        {
            var view = await GetOrCreate<TView>(viewSystem, type, modelFactory, skinTag, viewName);
            view.Show();
            return view;
        }

        public static async UniTask<TView> GetOrCreate<TView>(this IViewsLayout viewSystem,ViewType type, 
            Func<IViewModel> modelFactory,
            string skinTag = "", 
            string viewName = null)
            where TView : class, IView
        {
            var view     = viewSystem[type]?.Get<TView>();

            if (view != null)
                return view;
            
            var model = modelFactory();
            view = await Create<TView>(viewSystem,model, type, skinTag, viewName, null);
            return view;
        }

        public static async UniTask<TView> Create<TView>(this IViewsLayout viewSystem,IViewModel model,
            ViewType type,
            string skinTag = "",
            string viewName = null,
            Transform parent = null)
            where TView : class, IView
        {
            TView view = null;
            switch (type)
            {
                case ViewType.Screen:
                    view = await viewSystem.CreateScreen<TView>(model,skinTag,viewName);
                    break;
                case ViewType.Window:
                    view = await viewSystem.CreateWindow<TView>(model,skinTag,viewName);
                    break;
                case ViewType.Overlay:
                    view = await viewSystem.CreateOverlay<TView>(model,skinTag,viewName);
                    break;
                case ViewType.None:
                default:
                    view = await viewSystem.Create<TView>(model,skinTag,parent,viewName);
                    break;
            }

            return view;
        }

        public static async UniTask AwaitIsReadyAsync(this IView view)
        {
            if (view == null || view.ViewLifeTime.IsTerminated) return;
            var lifeTime = view.ViewLifeTime;
            await view.IsInitialized
                .Where(x => x)
                .FirstAsync(cancellationToken:lifeTime.Token);
        }

        public static async UniTask AwaitStatusAsync(this IView view,ViewStatus status)
        {
            if (view == null || view.ViewLifeTime.IsTerminated)
                return;
            
            var lifeTime = view.ViewLifeTime;
            
            await view.Status
                .Where(x => x == status)
                .FirstAsync(cancellationToken:lifeTime.Token);
        }

        public static async UniTask AwaitClose(this IView view)
        {
            while (view.LifeTime.IsTerminated == false)
                await UniTask.Yield();
        }
        
    }
}