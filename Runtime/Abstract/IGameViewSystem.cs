namespace UniGame.ViewSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using ViewType = UniModules.UniGame.UiSystem.Runtime.ViewType;

    public interface IGameViewSystem : 
        IDisposable, 
        IViewLayoutProvider,
        IViewModelResolver
    {
        void CloseAll();

        void CloseAll(ViewType viewType);

        UniTask<IView> OpenView(IViewModel viewModel, string viewType, string layout, string skinTag = "",
            string viewName = null);
        
        UniTask<T> InitializeView<T>(T view, IViewModel viewModel) where T : IView;
    }

}