namespace UniGame.ViewSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;

    public static class ViewLayoutProviderExtension
    {

        public static async UniTask<IView> OpenWindow(this IViewLayoutProvider provider, Type viewType, string skinTag = "",
            string viewName = null)
        {
            return await provider.OpenWindow(viewType.Name, skinTag, viewName);
        }
        public static async UniTask<IView> OpenScreen(this IViewLayoutProvider provider,Type viewType, string skinTag = "", string viewName = null)
        {
            return await provider.OpenScreen(viewType.Name, skinTag, viewName);
        }
        
        public static async UniTask<IView> OpenOverlay(this IViewLayoutProvider provider,Type viewType, string skinTag = "", string viewName = null)
        {
            return await provider.OpenOverlay(viewType.Name, skinTag, viewName);
        }
        public static async UniTask<IView> CreateWindow(this IViewLayoutProvider provider,Type viewType, 
            string skinTag = "", string viewName = null)
        {
            return await provider.CreateWindow(viewType.Name, skinTag, viewName);
        }
        public static async UniTask<IView> CreateScreen(this IViewLayoutProvider provider,Type viewType, string skinTag = "", 
            string viewName = null)
        {
            return await provider.CreateScreen(viewType.Name, skinTag, viewName);
        }
        public static async UniTask<IView> CreateOverlay(this IViewLayoutProvider provider,Type viewType, 
            string skinTag = "", string viewName = null)
        {
            return await provider.CreateOverlay(viewType.Name, skinTag, viewName);
        }
        
        public static async UniTask<IView> OpenWindow(this IViewLayoutProvider provider,IViewModel viewModel, 
            Type viewType, string skinTag = "", string viewName = null)
        {
            return await provider.OpenWindow(viewModel,viewType.Name, skinTag, viewName);
        }
        public static async UniTask<IView> OpenScreen(this IViewLayoutProvider provider,
            IViewModel viewModel, 
            Type viewType, string skinTag = "", string viewName = null)
        {
            return await provider.OpenScreen(viewModel,viewType.Name, skinTag, viewName);
        }
        public static async UniTask<IView> OpenOverlay(this IViewLayoutProvider provider,IViewModel viewModel, 
            Type viewType, string skinTag = "", string viewName = null)
        {
            return await provider.OpenOverlay(viewModel,viewType.Name, skinTag, viewName);
        }

        public static async UniTask<IView> CreateWindow(this IViewLayoutProvider provider,IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null)
        {
            return await provider.CreateWindow(viewModel,viewType.Name, skinTag, viewName);
        }
        public static async UniTask<IView> CreateScreen(this IViewLayoutProvider provider,IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null){
            return await provider.CreateScreen(viewModel,viewType.Name, skinTag, viewName);
        }
        public static async UniTask<IView> CreateOverlay(this IViewLayoutProvider provider,IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null){
            return await provider.CreateOverlay(viewModel,viewType.Name, skinTag, viewName);
        }
        
        public static async UniTask<T> OpenWindow<T>(this IViewLayoutProvider provider, IViewModel viewModel, string skinTag = "", string viewName = null)
            where T : class, IView
        {
            var window = await provider.OpenWindow(viewModel, typeof(T), skinTag, viewName) as T;
            return window;
        }
        
        public static async UniTask<T> OpenWindow<T>(this IViewLayoutProvider provider,  string skinTag = "", string viewName = null)
            where T : class, IView
        {
            var window = await provider.OpenWindow(typeof(T), skinTag, viewName) as T;
            return window;
        }
        
        public static async UniTask<T> OpenScreen<T>(this IViewLayoutProvider provider,string skinTag = "", string viewName = null)
            where T : class, IView
        {
            var window = await provider.OpenScreen(typeof(T), skinTag, viewName) as T;
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
        
        public static async UniTask<T> OpenOverlay<T>(this IViewLayoutProvider provider, string skinTag = "", string viewName = null)
            where T : class, IView
        {
            var window = await provider.OpenOverlay(typeof(T), skinTag, viewName) as T;
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