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
        
        public IView Bind(IView view, IViewModel model)
        {
            if (!HasBinding(view)) return view;

            return BindMethods(view, model);
        }
        
        public IView BindMethods(IView view, IViewModel model)
        {
            var modelType = model.GetType();
            var viewType = view.GetType();
            
            var modelFields = modelType.GetFields();

            foreach (var modelField in modelFields)
            {
                var method = viewType.GetMethod(modelField.Name, BindFlags);
                if(method == null) continue;
                if(method.ContainsGenericParameters) continue;
                var parameters = method.GetParametersInfo();
                if(parameters.Length > 1) continue;

                var value = modelField.GetValue(model);
                if(value == null) continue;

                Bind(view, value, method);
            }
            
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