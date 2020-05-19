namespace UniGame.UiSystem.Runtime
{
    using Abstracts;

    public interface IUiView<TViewModel> : IView 
        where TViewModel : class, IViewModel
    {
        TViewModel Model { get; }
    }
}