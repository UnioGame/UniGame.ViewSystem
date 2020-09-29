namespace UniGame.UiSystem.Runtime
{
    using UniModules.UniGame.UISystem.Runtime.Abstract;

    public interface IUiView<TViewModel> : IView
        where TViewModel : class, IViewModel
    {
        TViewModel    Model         { get; }
    }
}