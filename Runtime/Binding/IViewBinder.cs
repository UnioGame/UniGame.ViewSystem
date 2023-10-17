namespace UniGame.ViewSystem.Runtime.Binding
{

    public interface IViewBinder
    {
        public IView Bind(IView view,IViewModel model);
        
    }
}