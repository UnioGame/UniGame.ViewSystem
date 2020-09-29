namespace UniGame.UiSystem.Runtime
{
    using UniModules.UniGame.UISystem.Runtime.Abstract;

    public interface ILayoutFactoryView
    {
        void BindLayout(IViewLayoutProvider layouts);
    }
}