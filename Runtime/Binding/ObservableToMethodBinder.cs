namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Rx.Runtime.Extensions;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UniRx;

    [Serializable]
    public class ObservableToMethodBinder : IViewBinder
    {
        public static BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public |
                                               BindingFlags.NonPublic | BindingFlags.IgnoreCase;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasBinding(IView view)
        {
            var viewType = view.GetType();
            var hasAttribute = viewType.HasAttribute<ViewBindAttribute>();
            return hasAttribute;
        }
        
        public IView Bind(IView view)
        {
            return BindMethods(view, view.ViewModel);
        }
        
        public IView Bind(IView view, IViewModel model)
        {
            return BindMethods(view, model);
        }
        
        public IView BindMethods(IView view,IViewModel model)
        {
            if (model == null) return view;
            
            var modelType = model.GetType();
            
            var modelFields = modelType.GetFields();

            foreach (var modelField in modelFields)
                Bind(view, modelField.Name, modelField.Name);
            
            return view;
        }

        public IView Bind(IView view,string modelFieldName,string methodName)
        {
            var model = view.ViewModel;
            if (model == null) return view;
            
            var modelType = model.GetType();
            var viewType = view.GetType();
            var modelField = modelType.GetField(modelFieldName, BindFlags);
            if(modelField == null) return view;
            
            var method = viewType.GetMethod(methodName, BindFlags);
            if(method == null) return view;
                
            if(method.ContainsGenericParameters)  return view;
            
            var parameters = method.GetParametersInfo();
            if(parameters.Length > 1)  return view;

            var value = modelField.GetValue(model);
            if(value == null) return view;

            Bind(view, value, method);

            return view;
        }

        public IView Bind(IView view,object fieldValue,MethodInfo methodInfo)
        {
            var result = fieldValue switch
            {
                IObservable<Unit> value => view.Bind(value,methodInfo),
                _ => view
            };

            return result;
        }

        
    }
}