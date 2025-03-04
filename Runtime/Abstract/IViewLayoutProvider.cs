namespace UniGame.ViewSystem.Runtime
{
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.UiSystem.Runtime;

    public interface IViewLayoutProvider : 
        ILifeTimeContext, 
        IViewLayoutContainer,
        IViewElementFactory,
        IViewLayoutData
    {
        IViewModelTypeMap ModelTypeMap { get; }
        
        UniTask<IView> OpenWindow(string viewType, string skinTag = "", string viewName = null);
        UniTask<IView> OpenScreen(string viewType, string skinTag = "", string viewName = null);
        UniTask<IView> OpenOverlay(string viewType, string skinTag = "", string viewName = null);
        UniTask<IView> Open(string viewType, string layout, string skinTag = "", string viewName = null);
        
        UniTask<IView> CreateWindow(string viewType, string skinTag = "", string viewName = null);
        UniTask<IView> CreateScreen(string viewType, string skinTag = "", string viewName = null);
        UniTask<IView> CreateOverlay(string viewType, string skinTag = "", string viewName = null);
        
        UniTask<IView> OpenWindow(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null);
        UniTask<IView> OpenScreen(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null);
        UniTask<IView> OpenOverlay(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null);

        UniTask<IView> CreateWindow(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null);
        UniTask<IView> CreateScreen(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null);
        UniTask<IView> CreateOverlay(IViewModel viewModel, string viewType, string skinTag = "", string viewName = null);

        public UniTask<IView> Create(string viewType, ViewType layoutType, string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null);

        UniTask<IView> Create(string viewType, string layoutType,
            string skinTag = "",
            string viewName = null,
            ILifeTime ownerLifeTime = null);
    }
}