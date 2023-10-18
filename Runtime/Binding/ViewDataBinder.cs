namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Reflection;

    [Serializable]
    public class ViewDataBinder : IViewBinder
    {
        public static BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public |
                                               BindingFlags.NonPublic | BindingFlags.IgnoreCase;
        
        private ViewFieldBinder _fieldBinder = new ViewFieldBinder();
        private ObservableToMethodBinder _methodBinder = new ObservableToMethodBinder();
        
        public IView Bind(IView view, IViewModel model)
        {
            var viewGameObject = view.GameObject;
            if (viewGameObject == null) return view;

            var bindData = viewGameObject.GetComponent<ViewBindData>();
            if(bindData == null || bindData.isEnabled == false) return view;

            var sourceType = model.GetType();
            var viewBindData = bindData;

            foreach (var value in viewBindData.values)
            {
                if(value.value == null) continue;
                if(string.IsNullOrEmpty(value.field)) continue;

                var sourceField = sourceType.GetField(value.field, BindFlags);
                if(sourceField == null) continue;

                var connection = new BindDataConnection()
                {
                    source = model,
                    value = view,
                    sourceValue = sourceField.GetValue(model),
                    targetValue = value.value,
                };

                _fieldBinder.Bind(view, ref connection);
            }

            foreach (var bindValue in viewBindData.methods)
            {
                _methodBinder.Bind(view, bindValue.field,bindValue.method);
            }
            
            return view;
        }
    }
}