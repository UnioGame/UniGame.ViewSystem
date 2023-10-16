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
        
        public bool CheckField(Type viewType, Type modelType, FieldInfo viewField, FieldInfo modelField)
        {
            var viewFieldType = viewField.FieldType;
            var modelFieldType = modelField.FieldType;
            
            if (viewFieldType == modelFieldType)
                return true;
            
            if (viewFieldType.IsAssignableFrom(modelFieldType))
                return true;
            
            if (modelFieldType.IsAssignableFrom(viewFieldType))
                return true;

            return false;
        }
        
    }

    public interface IViewBinder
    {
        public IView BindToField(IView view,object viewField, object modelField);
        public IView BindFromField(IView view,object viewField, object modelField);
        public IView BindFromField(IView view,object modelField, MethodInfo viewMethod);
    }
}