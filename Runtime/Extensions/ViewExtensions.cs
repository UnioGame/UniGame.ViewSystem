using UniGame.UiSystem.Runtime.Abstracts;

public static class ViewExtensions
{

    public static IView AddTo(this IView parent, IView child)
    {
        parent.AddView(child);
        return parent;
    }
    
    public static IView CreateChild<TView>(this IView parent, IViewModel model)
        where TView : class, IView
    {
        parent.CreateView<TView>(model);
        return parent;
    }
    
}
