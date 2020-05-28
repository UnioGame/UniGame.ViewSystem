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
        UniTask<T> OpenWindow<T>(IViewModel viewModel, string skinTag = "")
            where T :class, IView;

        UniTask<T> OpenScreen<T>(IViewModel viewModel, string skinTag = "")
            where T :class, IView;

        UniTask<T> OpenOverlay<T>(IViewModel viewModel, string skinTag = "") 
            where T :class, IView;


        UniTask<IView> OpenWindow(IViewModel viewModel, Type viewType, string skinTag = "");

        UniTask<IView> OpenScreen(IViewModel viewModel, Type viewType, string skinTag = "");

        UniTask<IView> OpenOverlay(IViewModel viewModel, Type viewType, string skinTag = "");
    }
}