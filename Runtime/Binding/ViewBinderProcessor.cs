namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UnityEngine.Serialization;

    [Serializable]
    public class ViewBinderProcessor : IViewBinderProcessor,IViewBinder
    {
        public const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public |
                                                BindingFlags.NonPublic | BindingFlags.IgnoreCase;
        
        public List<IViewBinder> binders = new List<IViewBinder>()
        {
            new ViewUiFieldsBinder(),
            new ObservableToMethodBinder(),
        };
        
        public IView Bind(IView view, IViewModel model)
        {
            if (!HasBinding(view)) return view;

            foreach (var viewBinder in binders)
                viewBinder.Bind(view, model);
            
            return view;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasBinding(IView view)
        {
            var viewType = view.GetType();
            var hasAttribute = viewType.HasAttribute<ViewBindAttribute>();
            return hasAttribute;
        }
        
    }
}