namespace UniGame.ViewSystem.Runtime.Binding
{
    using System.Reflection;

    public interface IViewBinder
    {
        public IView BindField(IView view,ref FieldBindData bindData);
        
        public IView BindMethod(IView view,object modelField, MethodInfo viewMethod);
        
    }
}