using UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Abstract;

namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.UiSystem.Runtime;

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

        UniTask<IView> OpenEmptyView(ViewType viewType);
    }
}