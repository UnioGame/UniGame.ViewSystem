namespace UniModules.UniGame.UISystem.Runtime.Abstracts
{
    using global::UniGame.UiSystem.Runtime.Abstracts;
    using UniRx.Async;

    public interface IViewProvider
    {
        UniTask<T> CreateView<T>(IViewModel viewModel, string tag = null) where T : class, IView;
    }
}