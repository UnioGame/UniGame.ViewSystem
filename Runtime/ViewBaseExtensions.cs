namespace UniModules.UniGame.UISystem.Runtime
{
    using global::UniGame.UiSystem.Runtime;
    using global::UniGame.UiSystem.Runtime.Abstracts;
    using UniRx.Async;
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
        public static async UniTask<T> CreateView<T>(this ViewBase source, IViewModel viewModel, string skinTag = null, Transform parent = null, string viewName = null) 
            where T : class, IView
        {
            return await source.Layout.Create(viewModel, typeof(T), skinTag, parent, viewName) as T;
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
        public static async UniTask<T> OpenAsWindow<T>(this ViewBase source, IViewModel viewModel, string skinTag = null, string viewName = null) 
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
        public static async UniTask<T> OpenAsScreen<T>(this ViewBase source, IViewModel viewModel, string skinTag = null, string viewName = null) 
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
        public static async UniTask<T> OpenAsOverlay<T>(this ViewBase source, IViewModel viewModel, string skinTag = null, string viewName = null) 
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
        public static T GetView<T>(this ViewBase source) 
            where T : Component, IView
        {
            return source.Layout.Get<T>();
        }
    }
}