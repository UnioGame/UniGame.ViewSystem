namespace UniGame.ViewSystem.Runtime
{
    using Cysharp.Threading.Tasks;

    public static class ViewLayoutProviderExtension
    {
        public static async UniTask<T> OpenWindow<T>(this IViewLayoutProvider provider, IViewModel viewModel, string skinTag = "", string viewName = null)
            where T : class, IView
        {
            var window = await provider.OpenWindow(viewModel, typeof(T), skinTag, viewName) as T;
            return window;
        }

        public static async UniTask<T> OpenScreen<T>(this IViewLayoutProvider provider, IViewModel viewModel, string skinTag = "", string viewName = null)
            where T : class, IView
        {
            var window = await provider.OpenScreen(viewModel, typeof(T), skinTag, viewName) as T;
            return window;
        }

        public static async UniTask<T> OpenOverlay<T>(this IViewLayoutProvider provider, IViewModel viewModel, string skinTag = "", string viewName = null)
            where T : class, IView
        {
            var window = await provider.OpenOverlay(viewModel, typeof(T), skinTag, viewName) as T;
            return window;
        }
        
        public static async UniTask<T> CreateWindow<T>(this IViewLayoutProvider provider, IViewModel viewModel, string skinTag = "", string viewName = null)
            where T : class, IView
        {
            return await provider.CreateWindow(viewModel, typeof(T), skinTag, viewName) as T;
        }

        public static async UniTask<T> CreateScreen<T>(this IViewLayoutProvider provider, IViewModel viewModel, string skinTag = "", string viewName = null)
            where T : class, IView
        {
            return await provider.CreateScreen(viewModel, typeof(T), skinTag, viewName) as T;
        }

        public static async UniTask<T> CreateOverlay<T>(this IViewLayoutProvider provider, IViewModel viewModel, string skinTag = "", string viewName = null)
            where T : class, IView
        {
            return await provider.CreateOverlay(viewModel, typeof(T), skinTag, viewName) as T;
        }
    }
}