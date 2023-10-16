namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UniModules.UniCore.Runtime.ReflectionUtils;

    [Serializable]
    public class ViewBinderProcessor
    {
        private List<IViewBinder> _binders = new List<IViewBinder>();
        
        public IView Bind(IView view, IViewModel model)
        {
            var viewType = view.GetType();
            var modelType = model.GetType();
            
            var values = viewType.GetFields();
            var methods = viewType.GetMethods();
            var modelValues = modelType.GetFields();
            
            return view;
        }
        
        public ref BindField CheckField(object view, object model, FieldInfo viewField, FieldInfo modelField)
        {
            if(!viewField.Name.Equals(modelField.Name,StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
        
    }

    [Serializable]
    public struct BindField
    {
        public object viewField;
        public object valueField;
        
        public object view;
        public object value;
        
        public string name;
        
        public FieldInfo viewFieldInfo;
        public FieldInfo valueFieldInfo;
    }
    
    public interface IViewBinder
    {
        public IView BindToField(IView view,object viewField, object modelField);
        public IView BindFromField(IView view,object viewField, object modelField);
        public IView BindFromField(IView view,object modelField, MethodInfo viewMethod);
    }
}