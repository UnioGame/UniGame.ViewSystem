namespace UniGame.ViewSystem.Runtime
{
    using Cysharp.Threading.Tasks;

    public interface IViewProvider
    {
        UniTask<T> CreateView<T>(IViewModel viewModel, string tag = null) where T : class, IView;
    }
}