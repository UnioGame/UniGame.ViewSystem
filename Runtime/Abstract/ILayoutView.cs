using UniGame.UiSystem.Runtime;

namespace UniGame.ViewSystem.Runtime
{
    public interface ILayoutView:
        IModelView, 
        ILayoutItem
    {
        IView BindNested(ILayoutView view, IViewModel model);
    }
}