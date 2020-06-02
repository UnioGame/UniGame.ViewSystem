namespace UniModules.UniGame.UISystem.Runtime.Abstracts
{
    using global::UniGame.UiSystem.Runtime.Abstracts;
    using UniRx.Async;

    public interface IViewProvider
    {
        UniTask<T> CreateView<T>(IViewModel viewModel) where T : class, IView;
    }
}