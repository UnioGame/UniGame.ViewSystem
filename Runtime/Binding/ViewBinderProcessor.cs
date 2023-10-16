namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UnityEngine.Serialization;

    [Serializable]
    public class ViewBinderProcessor
    {
        private const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public |
                                                BindingFlags.NonPublic | BindingFlags.IgnoreCase;
        
        private List<IViewBinder> _binders = new List<IViewBinder>();
        
        public IView Bind(IView view, IViewModel model)
        {
            var viewType = view.GetType();
            var modelType = model.GetType();
            
            var viewFields = viewType.GetFields();
            var viewMethods = viewType.GetMethods();
            
            var modelFields = modelType.GetFields();
            var modelMethods = modelType.GetMethods();

            //bind fields
            foreach (var modelField in modelFields)
            {
                var modelValue = modelField.GetValue(model);
                var viewFiled = viewType.GetField(modelField.Name, BindFlags);
                var methodInfo = viewType.GetMethod(modelField.Name, BindFlags);

                if (viewFiled != null)
                {
                    var viewValue = viewFiled.GetValue(view);
                    
                    var bindData = new FieldBindData()
                    {
                        source = model,
                        target = view,
                        sourceField = modelField,
                        targetField = viewFiled,
                        sourceValue = modelValue,
                        targetValue = viewValue,
                    };

                    BindField(ref bindData);
                }

                if (methodInfo != null)
                {
                    
                }
                
                
            }
            
            return view;
        }
        
        public void BindField(ref FieldBindData bindData)
        {
            foreach (var viewBinder in _binders)
                viewBinder.BindField(ref bindData);
        }
        
    }

    [Serializable]
    public struct FieldBindData
    {
        public object source;
        public object target;

        public FieldInfo sourceField;
        public FieldInfo targetField;
        
        public object sourceValue;
        public object targetValue;
    }
    
    [Serializable]
    public struct MethodBindFieldData
    {
        public object source;
        public object target;

        public FieldInfo sourceField;
        public FieldInfo targetField;
    }
}