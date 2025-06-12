namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [Serializable]
    public class ViewBinderProcessor : IViewBinderProcessor,IViewBinder
    {
        public const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public |
                                                BindingFlags.NonPublic | BindingFlags.IgnoreCase;
        
        public List<IViewBinder> binders = new()
        {
            new ViewUiFieldsBinder(),
            new ViewDataBinder()
        };
        
        public IView Bind(IView view, IViewModel model)
        {
            foreach (var viewBinder in binders)
                viewBinder.Bind(view, model);
            
            return view;
        }


        
    }
}