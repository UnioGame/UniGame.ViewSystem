namespace UniGame.ViewSystem.Runtime
{
    using System;
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Abstract;

    public interface IViewLayoutProvider : 
        ILifeTimeContext, 
        IViewLayoutContainer,
        IViewElementFactory,
        IViewLayoutData
    {
        IViewModelTypeMap ModelTypeMap { get; }
        
        UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> OpenScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> OpenOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);

        UniTask<IView> CreateWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> CreateScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
        UniTask<IView> CreateOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
    }
}