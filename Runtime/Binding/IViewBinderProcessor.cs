namespace UniGame.ViewSystem.Runtime.Binding
{
    public interface IViewBinderProcessor
    {
        IView Bind(IView view, IViewModel model);
    }
}