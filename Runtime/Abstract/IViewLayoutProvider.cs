namespace UniGame.ViewSystem.Runtime
{
    using System;
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using Runtime.Abstract;

    public interface IViewLayoutProvider : 
        ILifeTimeContext, 
        IViewLayoutContainer,
        IViewElementFactory,
        IViewLayoutData
    {
        IViewModelTypeMap ModelTypeMap { get; }
        
        UniTask<IView> OpenWindow(Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> OpenScreen(Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> OpenOverlay(Type viewType, string skinTag = "", string viewName = null);

        UniTask<IView> CreateWindow(Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> CreateScreen(Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> CreateOverlay(Type viewType, string skinTag = "", string viewName = null);
        
        UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> OpenScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> OpenOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);

        UniTask<IView> CreateWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> CreateScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> CreateOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
    }
}