namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UniGame.Runtime.ReflectionUtils;

    [Serializable]
    public class ViewUiFieldsBinder : IViewBinder
    {
        public static BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public |
                                              BindingFlags.NonPublic | BindingFlags.IgnoreCase;
        
        public ViewFieldBinder fieldBinder = new();
        public ObservableToMethodBinder methodBinder = new();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasBinding(IView view)
        {
            var viewType = view.GetType();
            var hasAttribute = viewType.HasAttribute<ViewBindAttribute>();
            return hasAttribute;
        }
        
        public IView Bind(IView view,IViewModel model)
        {
            if (!HasBinding(view)) return view;

            BindField(view, model);

            methodBinder.Bind(view, model);
            
            return view;
        }

        public IView BindField(IView view,IViewModel model)
        {
            var viewType = view.GetType();
            var modelType = model.GetType();
            
            var modelFields = modelType.GetFields();

            //bind fields
            foreach (var modelField in modelFields)
            {
                var modelValue = modelField.GetValue(model);
                var viewFiled = viewType.GetField(modelField.Name, BindFlags);

                if(viewFiled == null) continue;
                
                var viewValue = viewFiled.GetValue(view);
                
                var bindData = new BindDataConnection()
                {
                    source = model,
                    value = view,
                    sourceValue = modelValue,
                    targetValue = viewValue,
                };
                
                fieldBinder.Bind(view, ref bindData);
            }
            
            
            return view;
        }
        
    }
}