using UniGame.UiSystem.Runtime;

namespace UniGame.ViewSystem.Runtime
{
    using Cysharp.Threading.Tasks;

    public interface ILayoutView:
        IModelView, 
        ILayoutItem
    {
        UniTask<IView> BindNested(ILayoutView view, IViewModel model);
    }
}