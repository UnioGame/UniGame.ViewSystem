namespace UniModules.UniGame.UISystem.Runtime.Extensions
{
    using System;
    using Abstract;
    using Core.Runtime.DataFlow.Interfaces;
    using Cysharp.Threading.Tasks;
    using global::UniGame.UiSystem.Runtime;
    using UniRx;
    using UnityEngine;

    public static class ViewBaseExtensions
    {
        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="viewModel"></param>
        /// <param name="skinTag"></param>
        /// <param name="parent"></param>
        /// <param name="viewName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> CreateViewAsync<T>(
            this ViewBase source, 
            IViewModel viewModel, 
            string skinTag, 
            Transform parent = null, 
            string viewName = null) 
            where T : class, IView
        {
            return await source.Layout.Create(viewModel, typeof(T), skinTag, parent, viewName) as T;
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateHiddenAsync<T>(
            this ViewBase source, 
            IViewModel viewModel, 
            string skinTag = null, 
            Transform parent = null, 
            string viewName = null) 
            where T : class, IView
        {
            var view = await source.Layout.Create(viewModel, typeof(T), skinTag, parent, viewName) as T;
            view.Hide();
            return view;
        }


        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateNestedViewAsync<T>(
            this ViewBase source,
            IViewModel viewModel,
            string skinTag = null,
            Transform parent = null,
            string viewName = null,
            bool stayWorld = false) 
            where T : class, IView
        {
            return await CreateNestedViewAsync<T>(source, source.LifeTime, viewModel, skinTag, parent, viewName);
        }
        
        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateNestedViewForModelAsync<T>(
            this ViewBase source, 
            IViewModel viewModel, 
            string skinTag, 
            Transform parent = null, 
            string viewName = null) 
            where T : class, IView
        {
            return await CreateNestedViewAsync<T>(source, source.ModelLifeTime, viewModel, skinTag, parent, viewName);
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateNestedViewForModelAsync<T>(
            this ViewBase source,
            IViewModel viewModel,
            Transform parent = null,
            bool stayWorld = false)
            where T : class, IView
        {
            return await CreateNestedViewAsync<T>(source, source.ModelLifeTime, viewModel, parent, stayWorld);
        }
        
        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> ShowNestedViewAsync<T>(
            this ViewBase source,
            IViewModel viewModel,
            Transform parent = null,
            bool stayWorld = false)
            where T : class, IView
        {
            var view = await CreateNestedViewAsync<T>(source, source.ModelLifeTime, viewModel, parent, stayWorld);
            view.Show();
            return view;
        }
        
        
        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateNestedViewAsync<T>(
            this ViewBase source,
            IViewModel viewModel,
            Transform parent = null,
            bool stayWorld = false)
            where T : class, IView
        {
            return await CreateNestedViewAsync<T>(source, source.LifeTime, viewModel, parent, stayWorld);
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateNestedViewAsync<T>(
            this ViewBase source, 
            ILifeTime lifeTime,
            IViewModel viewModel, 
            Transform parent = null,
            bool stayWorld = false,
            string skinTag = null,
            string viewName = null) 
            where T : class, IView
        {
            return await CreateNestedViewAsync<T>(source, lifeTime, viewModel, skinTag, parent, viewName, stayWorld);
        }
        
        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateViewAsync<T>(
            this ViewBase source, 
            IViewModel viewModel, 
            Transform parent = null, 
            string skinTag = null, 
            string viewName = null,
            bool stayWorld = false) 
            where T : class, IView
        {
            var view = await source.Layout.Create(viewModel, typeof(T), skinTag, parent, viewName,stayWorld) as T;
            return view;
        }
        
        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> ShowViewAsync<T>(
            this ViewBase source, 
            IViewModel viewModel, 
            Transform parent = null, 
            string skinTag = null, 
            string viewName = null,
            bool stayWorld = false) 
            where T : class, IView
        {
            var view = await source.Layout.Create(viewModel, typeof(T), skinTag, parent, viewName,stayWorld) as T;
            view.Show();
            return view;
        }
        
        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateNestedViewAsync<T>(
            this ViewBase source, 
            ILifeTime lifeTime,
            IViewModel viewModel, 
            string skinTag = null, 
            Transform parent = null, 
            string viewName = null,
            bool stayWorld = false) 
            where T : class, IView
        {
            var view = await source.Layout.Create(viewModel, typeof(T), skinTag, parent, viewName,stayWorld) as T;
            view.CloseWith(lifeTime);
            return view;
        }

        /// <summary>
        /// Create a new view and open it as window (see <see cref="IViewLayoutProvider.OpenWindow"/>).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="viewModel"></param>
        /// <param name="skinTag"></param>
        /// <param name="viewName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> OpenAsWindowAsync<T>(this ViewBase source, IViewModel viewModel, string skinTag = null, string viewName = null) 
            where T : class, IView
        {
            return await source.Layout.OpenWindow(viewModel, typeof(T), skinTag, viewName) as T;
        }

        /// <summary>
        /// Create a new view and open it as screen (see <see cref="IViewLayoutProvider.OpenScreen"/>).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="viewModel"></param>
        /// <param name="skinTag"></param>
        /// <param name="viewName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> OpenAsScreenAsync<T>(this ViewBase source, IViewModel viewModel, string skinTag = null, string viewName = null) 
            where T : class, IView
        {
            return await source.Layout.OpenScreen(viewModel, typeof(T), skinTag, viewName) as T;
        }

        /// <summary>
        /// Create a new view and open it as overlay (see <see cref="IViewLayoutProvider.OpenOverlay"/>).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="viewModel"></param>
        /// <param name="skinTag"></param>
        /// <param name="viewName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> OpenAsOverlayAsync<T>(this ViewBase source, IViewModel viewModel, string skinTag = null, string viewName = null) 
            where T : class, IView
        {
            return await source.Layout.OpenOverlay(viewModel, typeof(T), skinTag, viewName) as T;
        }

        /// <summary>
        /// Get an existing view by type (T).
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetView<T>(this ViewBase source) where T : Component, IView
        {
            return source.Layout.Get<T>();
        }

        public static T DestroyWith<T>(this T view, ILifeTime lifeTime) where T : IView
        {
            lifeTime.AddCleanUpAction(view.Destroy);
            return view;
        }
        
        public static T CloseWith<T>(this T view, ILifeTime lifeTime) where T : IView
        {
            lifeTime.AddCleanUpAction(view.Close);
            return view;
        }
        
        public static T HideWith<T>(this T view, ILifeTime lifeTime) where T : IView
        {
            lifeTime.AddCleanUpAction(view.Hide);
            return view;
        }
        
        public static T ShowWith<T>(this T view, ILifeTime lifeTime) where T : IView
        {
            lifeTime.AddCleanUpAction(view.Show);
            return view;
        }
        
        /// <summary>
        /// Add new child view to active view item.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="view"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns>Return current view.</returns>
        public static void AddAsChild<T>(this ViewBase source, T view, bool worldPositionStays = false) where T : ViewBase
        {
            var sourceTransform = source.transform;

            AddAsChild(source, view, sourceTransform, worldPositionStays);
        }

        /// <summary>
        /// Add new child view to active view item.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="view"></param>
        /// <param name="newParent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns>Return current view.</returns>
        public static void AddAsChild<T>(this ViewBase source, T view, Transform newParent, bool worldPositionStays = false) where T : ViewBase
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (view == null)
                throw new ArgumentNullException(nameof(source));
            
            var viewTransform  = view.transform;

            if (viewTransform == null || viewTransform.parent == newParent)
                throw new InvalidOperationException("Cannot add view as a child because it's the parent!");

            view.Owner.layer = source.Owner.layer;
            viewTransform.SetParent(newParent, worldPositionStays);
            viewTransform.localScale = Vector3.one;
        }

        public static IObservable<TSource> OnHidden<TSource>(this TSource source)where TSource : IViewStatus 
            => GetObservable(source, ViewStatus.Hidden);
        public static IObservable<TSource> OnBeginHide<TSource>(this TSource source) where TSource : IViewStatus 
            => GetObservable(source, ViewStatus.Hiding);
        public static IObservable<TSource> OnBeginShow<TSource>(this TSource source) where TSource : IViewStatus
            => GetObservable(source, ViewStatus.Showing);
        public static IObservable<TSource> OnShown<TSource>(this TSource source) where TSource : IViewStatus
            => GetObservable(source, ViewStatus.Shown);
        public static IObservable<TSource> OnClosed<TSource>(this TSource source) where TSource : IViewStatus 
            => GetObservable(source, ViewStatus.Closed);

        public static IObservable<TSource> GetObservable<TSource>(this TSource source, ViewStatus status) where TSource : IViewStatus
            =>  source.Status.Where(x => x == status).Select(x => source);
    }
}