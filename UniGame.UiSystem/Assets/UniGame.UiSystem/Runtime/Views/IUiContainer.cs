namespace UniGame.UiSystem.Runtime
{
    using Abstracts;

    public interface IUiContainer
    {
        void Add(IView view);
        void Remove(IView view);
    }
}