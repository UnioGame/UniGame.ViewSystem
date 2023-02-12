using UniGame.ViewSystem.Runtime;

namespace UniGame.ViewSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IGameViewSystem : 
        IDisposable, 
        IViewLayoutProvider,
        IViewModelResolver
    {
        void CloseAll();

        UniTask<T> InitializeView<T>(T view, IViewModel viewModel) where T : IView;
    }

}