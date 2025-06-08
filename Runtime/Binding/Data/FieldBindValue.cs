namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UniGame.Runtime.ReflectionUtils;
    using UnityEngine;
    using UnityEngine.Pool;
    using Object = UnityEngine.Object;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public class FieldBindValue
    {
        private const string SettingsTabKey = "Settings";
        private const string FieldTabKey = "Field";
        
        private static BindingFlags FieldBindingFlags = 
            BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.NonPublic | BindingFlags.IgnoreCase;
        
#if ODIN_INSPECTOR
        [TabGroup(FieldTabKey)]
        [EnableIf(nameof(enabled))]
#endif
        public GameObject target;
        
#if ODIN_INSPECTOR
        [TabGroup(FieldTabKey)]
        [ValueDropdown(nameof(GetComponents))]
        [HideLabel]
        [EnableIf(nameof(enabled))]
#endif
        public Object value;
        
#if ODIN_INSPECTOR
        [TabGroup(FieldTabKey)]
        [EnableIf(nameof(enabled))]
        [ValueDropdown(nameof(GetFields))]
        [HideLabel]
        [OnValueChanged(nameof(OnFieldChanged))]
#endif
        public string field = string.Empty;
        
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
#endif
        public bool enabled;
        
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
#endif
        public ViewBindData view;
        
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
#endif
        public string label;
        
#if ODIN_INSPECTOR
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

        public IEnumerable<ValueDropdownItem<string>> GetFields()
        {
            if(view == null) yield break;

            var modelType = view.ModelType;
            var fields = modelType.GetFields();

            foreach (var fieldInfo in fields)
            {
                yield return new ValueDropdownItem<string>()
                {
                    Text = GetFieldLabel(fieldInfo),
                    Value = fieldInfo.Name
                };
            }
        }
#endif
        
#if ODIN_INSPECTOR
        [OnInspectorInit]
#endif
        private void OnFieldChanged()
        {
            if(view == null) return;
            var modelType = view.ModelType;
            var fieldInfo = modelType.GetField(field,FieldBindingFlags);
            label = GetFieldLabel(fieldInfo);
        }
        
        public string GetFieldLabel(FieldInfo fieldInfo)
        {
            if (fieldInfo == null) return string.Empty;
            return $"{fieldInfo.Name}   :{fieldInfo.FieldType.GetFormattedName()}";
        }
    }
}