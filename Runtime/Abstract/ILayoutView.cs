using UniGame.UiSystem.Runtime;

namespace UniGame.ViewSystem.Runtime
{
    public interface ILayoutView:
        IView, 
        ILayoutItem
    {
        IView BindNested(ILayoutView view, IViewModel model);
    }
}