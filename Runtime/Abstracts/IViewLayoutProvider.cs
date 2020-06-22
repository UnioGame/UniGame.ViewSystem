namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx.Async;

    public interface IViewLayoutProvider : 
        ILifeTimeContext, 
        IViewLayoutContainer,
        IViewElementFactory
    {
        UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);

        UniTask<IView> OpenScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);

        UniTask<IView> OpenOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
    }

    public static class IViewLayoutProviderExtension
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
    }
}