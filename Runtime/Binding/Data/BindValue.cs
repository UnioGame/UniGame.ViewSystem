namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UnityEngine;
    using UnityEngine.Pool;
    using Object = UnityEngine.Object;

    [Serializable]
    public class BindValue
    {
        private const string SettingsTabKey = "Settings";
        private const string FieldTabKey = "Field";
        
        [TabGroup(FieldTabKey)]
        [EnableIf(nameof(enabled))]
        public GameObject target;
        
        [TabGroup(FieldTabKey)]
        [ValueDropdown(nameof(GetComponents))]
        [HideLabel]
        [EnableIf(nameof(enabled))]
        public Object value;
        
        [TabGroup(FieldTabKey)]
        [EnableIf(nameof(enabled))]
        [ValueDropdown(nameof(GetFields))]
        [HideLabel]
        public string field = string.Empty;
        
        [TabGroup(SettingsTabKey)]
        public bool enabled;
        
        [TabGroup(SettingsTabKey)]
        public GameObject view;
        
        public IEnumerable<ValueDropdownItem<Object>> GetComponents()
        {
            if(target == null) yield break;

            yield return new ValueDropdownItem<Object>()
            {
                Text = target.GetType().Name,
                Value = target,
            };
            
            var components = ListPool<Component>.Get();
            target.GetComponents(components);

            foreach (var componentValue in components)
            {
                yield return new ValueDropdownItem<Object>()
                {
                    Text = componentValue.GetType().Name,
                    Value = componentValue,
                };
            }
            
            components.Despawn();
        }

        public IEnumerable<string> GetFields()
        {
            if(view == null) yield break;
            
            var viewItem = view.GetComponent<IView>();
            if(viewItem == null) yield break;

            var modelType = viewItem.ModelType;
            var fields = modelType.GetFields();
            
            foreach (var fieldInfo in fields)
                yield return fieldInfo.Name;
        }
        
    }
}