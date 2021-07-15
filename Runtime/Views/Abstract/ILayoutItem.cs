namespace UniGame.UiSystem.Runtime
{
    using UniModules.UniGame.UISystem.Runtime.Abstract;

    public interface ILayoutItem
    {
        void BindLayout(IViewLayoutProvider layouts);
    }
}