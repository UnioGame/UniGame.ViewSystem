namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using Cysharp.Threading.Tasks;

    public interface IViewProvider
    {
        UniTask<T> CreateView<T>(IViewModel viewModel, string tag = null) where T : class, IView;
    }
}