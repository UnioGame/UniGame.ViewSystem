using UniGame.UiSystem.Runtime;

namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    public interface ILayoutView:
        IView, 
        ILayoutItem
    {
        IView BindNested(ILayoutView view, IViewModel model);
    }
}