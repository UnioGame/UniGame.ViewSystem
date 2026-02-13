namespace UniGame.ViewSystem.Runtime
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using global::UniGame.UiSystem.Runtime;
    using R3;
    using UnityEngine;

    public static class ViewsExtensions
    {
        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="viewModel"></param>
        /// <param name="skinTag"></param>
        /// <param name="parent"></param>
        /// <param name="viewName"></param>
        /// <param name="stayWorld"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> CreateViewAsync<T>(
            this IView source,
            IViewModel viewModel,
            string skinTag = null,
            Transform parent = null,
            string viewName = null,
            bool stayWorld = false)
            where T : class, IView
        {
            return await source.Layout.Create(viewModel, typeof(T).Name, skinTag, parent, viewName, stayWorld) as T;
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateHiddenAsync<T>(
            this IView source,
            IViewModel viewModel,
            string skinTag = null,
            Transform parent = null,
            string viewName = null)
            where T : class, IView
        {
            var view = await source.Layout.Create(viewModel, typeof(T).Name, skinTag, parent, viewName);
            view.Hide();
            return view as T;
        }


        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateNestedViewAsync<T>(
            this IView source,
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
            this IView source,
            IViewModel viewModel,
            string skinTag,
            Transform parent = null,
            string viewName = null)
            where T : class, IView
        {
            return await CreateNestedViewAsync<T>(source, source.LifeTime, viewModel, skinTag, parent, viewName);
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateNestedViewForModelAsync<T>(
            this IView source,
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
        public static async UniTask<T> ShowNestedViewAsync<T>(
            this IView source,
            IViewModel viewModel,
            Transform parent = null,
            bool stayWorld = false)
            where T : class, IView
        {
            var view = await CreateNestedViewAsync<T>(source, source.LifeTime, viewModel, parent, stayWorld);
            view.Show();
            return view;
        }


        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateNestedViewAsync<T>(
            this IView source,
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
            this IView source,
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
        public static async UniTask<T> ShowViewAsync<T>(
            this IView source,
            IViewModel viewModel,
            Transform parent = null,
            string skinTag = null,
            string viewName = null,
            bool stayWorld = false)
            where T : class, IView
        {
            var view = await source.ShowViewAsync(viewModel, typeof(T), parent, skinTag, viewName, stayWorld) as T;
            return view;
        }

        public static async UniTask<IView> ShowViewAsync(
            this IView source,
            IViewModel viewModel,
            Type viewType,
            Transform parent = null,
            string skinTag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            var view = await source.Layout.Create(viewModel, viewType.Name, skinTag, parent, viewName, stayWorld);
            view.Show();
            return view;
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// And close it with target lifetime
        /// </summary>
        public static async UniTask<T> CreateNestedViewAsync<T>(
            this IView source,
            ILifeTime lifeTime,
            IViewModel viewModel,
            string skinTag = null,
            Transform parent = null,
            string viewName = null,
            bool stayWorld = false)
            where T : class, IView
        {
            var view = await CreateNestedViewAsync(source, lifeTime, viewModel, typeof(T), skinTag, parent, viewName,
                stayWorld) as T;
            return view;
        }

        public static async UniTask<IView> CreateNestedViewAsync(
            this IView source,
            ILifeTime lifeTime,
            IViewModel viewModel,
            Type viewType,
            string skinTag = null,
            Transform parent = null,
            string viewName = null,
            bool stayWorld = false)
        {
            var view = await source.Layout.Create(viewModel, viewType.Name, skinTag, parent, viewName, stayWorld);
            view.CloseWith(lifeTime);
            return view;
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> CreateChildViewAsync<T>(
            this IView source,
            IViewModel viewModel,
            Transform parent = null,
            string skinTag = null,
            string viewName = null,
            bool stayWorld = false,
            ILifeTime lifeTime = null)
            where T : class, IView
        {
            var view = await source
                .CreateChildViewAsync(viewModel, typeof(T), parent, skinTag, viewName, stayWorld, lifeTime) as T;
            return view;
        }

        public static async UniTask<IView> CreateChildViewAsync(
            this IView source,
            IViewModel viewModel,
            Type viewType,
            Transform parent = null,
            string skinTag = null,
            string viewName = null,
            bool stayWorld = false,
            ILifeTime lifeTime = null)
        {
            parent = parent ? parent : source.Transform;
            var view = await source.Layout
                .Create(viewModel, viewType.Name, skinTag, parent, viewName, stayWorld, lifeTime);

            var viewTransform = view.Transform;
            view.Owner.layer = source.Owner.layer;
            viewTransform.localScale = Vector3.one;

            return view;
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<IView> CreateChildViewAsync(
            this IView source,
            Type viewType,
            IViewModel viewModel,
            Transform parent = null,
            string skinTag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            parent = parent ? parent : source.Transform;

            var view = await source.Layout.Create(viewModel, viewType.Name,
                skinTag, parent, viewName, stayWorld);

            var viewTransform = view.Transform;
            view.Owner.layer = source.Owner.layer;
            viewTransform.localScale = Vector3.one;

            return view;
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<T> ShowChildViewAsync<T>(
            this IView source,
            IViewModel viewModel,
            Transform parent = null,
            string skinTag = null,
            string viewName = null,
            bool stayWorld = false,
            ILifeTime lifeTime = null)
            where T : class, IView
        {
            var view = await source.CreateChildViewAsync<T>(viewModel, parent, skinTag, viewName, stayWorld, lifeTime);
            await view.ShowAsync();
            return view;
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<IView> ShowChildViewAsync(
            this IView source,
            Type viewType,
            Transform parent = null,
            string skinTag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            var view = await CreateChildViewAsync(source, viewType, parent, skinTag, viewName, stayWorld);
            view.Show();
            return view;
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<TView> ShowChildViewAsync<TView>(
            this IView source,
            Transform parent = null,
            string skinTag = null,
            string viewName = null,
            bool stayWorld = false)
            where TView : class, IView
        {
            var viewType = typeof(TView);
            var view = await CreateChildViewAsync(source, viewType, parent, skinTag, viewName, stayWorld);
            view.Show();
            return view as TView;
        }

        public static async UniTask<TView> CreateChildViewAsync<TView>(
            this IView source,
            Transform parent = null,
            string skinTag = null,
            string viewName = null,
            bool stayWorld = false)
            where TView : class, IView
        {
            var viewType = typeof(TView);
            var view = await CreateChildViewAsync(source, viewType, parent, skinTag, viewName, stayWorld);
            return view as TView;
        }

        /// <summary>
        /// Create a new view (see <see cref="IView"/> or <see cref="ViewBase"/>) with view model (see <see cref="IViewModel"/>).
        /// </summary>
        public static async UniTask<IView> CreateChildViewAsync(
            this IView source,
            Type viewType,
            Transform parent = null,
            string skinTag = null,
            string viewName = null,
            bool stayWorld = false)
        {
            parent = parent ? parent : source.Transform;

            var view = await source.Layout
                .Create(viewType, parent, skinTag, viewName, stayWorld);

            var viewTransform = view.Transform;
            view.Owner.layer = source.Owner.layer;
            viewTransform.localScale = Vector3.one;

            return view;
        }

        /// <summary>
        /// Create a new view and open it as window (see <see cref="IViewsLayout.OpenWindow"/>).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="viewModel"></param>
        /// <param name="skinTag"></param>
        /// <param name="viewName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> OpenAsWindowAsync<T>(this IView source, IViewModel viewModel,
            string skinTag = null, string viewName = null)
            where T : class, IView
        {
            return await source.Layout.OpenWindow(viewModel, typeof(T), skinTag, viewName) as T;
        }

        /// <summary>
        /// Create a new view and open it as window (see <see cref="IViewsLayout.OpenWindow"/>).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewType"></param>
        /// <param name="skinTag"></param>
        /// <param name="viewName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<IView> OpenAsWindowAsync(this IView source,
            IViewModel viewModel, Type viewType, string skinTag = null, string viewName = null)
        {
            return await source.Layout.OpenWindow(viewModel, viewType, skinTag, viewName);
        }

        /// <summary>
        /// Create a new view and open it as screen (see <see cref="IViewsLayout.OpenScreen"/>).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="viewModel"></param>
        /// <param name="skinTag"></param>
        /// <param name="viewName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> OpenAsScreenAsync<T>(this IView source, IViewModel viewModel,
            string skinTag = null, string viewName = null)
            where T : class, IView
        {
            return await source.Layout.OpenScreen(viewModel, typeof(T), skinTag, viewName) as T;
        }

        /// <summary>
        /// Create a new view and open it as overlay (see <see cref="IViewsLayout.OpenOverlay"/>).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="viewModel"></param>
        /// <param name="skinTag"></param>
        /// <param name="viewName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> OpenAsOverlayAsync<T>(this IView source, IViewModel viewModel,
            string skinTag = null, string viewName = null)
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
        public static T GetView<T>(this IView source) where T : Component, IView
        {
            return source.Layout.GetView<T>();
        }

        public static T DestroyWith<T>(this T view, ILifeTime lifeTime) where T : IView
        {
            lifeTime.AddCleanUpAction(view.Destroy);
            return view;
        }


        public static TView CloseWithModel<TView>(this TView view)
            where TView : IView
        {
            if (view.LifeTime.IsTerminated || view.ViewModel == null) return view;
            var viewModel = view.ViewModel;
            view.CloseWith(viewModel.LifeTime);
            return view;
        }

        public static async UniTask<T> CloseWith<T>(this UniTask<T> viewTask, ILifeTime lifeTime)
            where T : IView
        {
            var view = await viewTask;
            if (view != null)
                lifeTime.AddCleanUpAction(view.Close);

            return view;
        }

        public static T CloseWith<T>(this T view, ILifeTime lifeTime) where T : IView
        {
            if (view != null)
            {
                lifeTime.AddCleanUpAction(view.Close);
            }

            return view;
        }

        public static T HideWith<T>(this T view, ILifeTime lifeTime) where T : IView
        {
            if (view != null)
            {
                lifeTime.AddCleanUpAction(view.Hide);
            }

            return view;
        }

        public static T ShowWith<T>(this T view, ILifeTime lifeTime) where T : IView
        {
            if (view != null)
            {
                lifeTime.AddCleanUpAction(view, x => x.Show());
            }

            return view;
        }

        /// <summary>
        /// Add new child view to active view item.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="view"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns>Return current view.</returns>
        public static void AddAsChild<T>(this IView source, T view, bool worldPositionStays = false)
            where T : ViewBase
        {
            var sourceTransform = source.Transform;
            source.AddAsChild(view, sourceTransform, worldPositionStays);
        }

        /// <summary>
        /// Add new child view to active view item.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="view"></param>
        /// <param name="newParent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns>Return current view.</returns>
        public static void AddAsChild<T>(this IView source, T view, Transform newParent,
            bool worldPositionStays = false)
            where T : IView
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (view == null)
                throw new ArgumentNullException(nameof(source));

            var viewTransform = view.Transform;

            if (viewTransform == null)
                throw new InvalidOperationException("Cannot add view as a child because it's the parent!");

            view.Owner.layer = source.Owner.layer;
            viewTransform.SetParent(newParent, worldPositionStays);
            viewTransform.localScale = Vector3.one;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Observable<TSource> OnHidden<TSource>(this TSource source) where TSource : IViewStatus
            => source.GetObservable(ViewStatus.Hidden);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Observable<TSource> OnBeginHide<TSource>(this TSource source) where TSource : IViewStatus
            => source.GetObservable(ViewStatus.Hiding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Observable<TSource> OnBeginShow<TSource>(this TSource source) where TSource : IViewStatus
            => source.GetObservable(ViewStatus.Showing);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Observable<TSource> OnShown<TSource>(this TSource source) where TSource : IViewStatus
            => source.GetObservable(ViewStatus.Shown);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Observable<TSource> OnClosed<TSource>(this TSource source) where TSource : IViewStatus
            => source.GetObservable(ViewStatus.Closed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Observable<TSource> GetObservable<TSource>(this TSource source, ViewStatus status)
            where TSource : IViewStatus
        {
            return source.Status
                .Where(status, static (x, y) => x == y)
                .Select(source, static (x, y) => y);
        }
        
        public static async UniTask<IView> RegisterView(this IView view,CancellationToken cancellationToken = default)
        {
            var viewSystem = GameViewSystem.ViewSystem;
            if (viewSystem == null || view == null) return view;

            var viewModel = await viewSystem.CreateViewModel(view.GetType().Name);
            return await view.RegisterView(viewModel,null).AttachExternalCancellation<IView>(cancellationToken);
        }

        public static async UniTask<IView> RegisterView(this IView view, IViewModel viewModel, IViewLayout layout = null)
        {
            var viewSystem = GameViewSystem.ViewSystem;
            if (viewSystem == null) return view;
            
            await viewSystem.InitializeView(view, viewModel, layout);
            return view;
        }
    }
}