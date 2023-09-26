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

        UniTask<T> InitializeView<T>(T view, IViewModel viewModel) where T : IView;
    }

}