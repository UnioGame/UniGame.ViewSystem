namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IViewLayoutProvider : 
        ILifeTimeContext, 
        IViewLayoutContainer,
        IViewElementFactory,
        IViewLayoutData
    {
        UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);

        UniTask<IView> OpenScreen(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);

        UniTask<IView> OpenOverlay(IViewModel viewModel, Type viewType, string skinTag = "", string viewName = null);
    }
}