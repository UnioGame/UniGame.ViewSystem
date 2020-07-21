namespace UniModules.UniGame.UISystem.Runtime.Abstracts
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.UiSystem.Runtime.Abstracts;
    

    public interface IViewProvider
    {
        UniTask<T> CreateView<T>(IViewModel viewModel, string tag = null) where T : class, IView;
    }
}