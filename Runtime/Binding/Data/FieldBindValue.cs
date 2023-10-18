namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Sirenix.OdinInspector;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UnityEngine;
    using UnityEngine.Pool;
    using Object = UnityEngine.Object;

    [Serializable]
    public class FieldBindValue
    {
        private const string SettingsTabKey = "Settings";
        private const string FieldTabKey = "Field";
        
        private static BindingFlags FieldBindingFlags = 
            BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.NonPublic | BindingFlags.IgnoreCase;
        
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
        [OnValueChanged(nameof(OnFieldChanged))]
        public string field = string.Empty;
        
        [TabGroup(SettingsTabKey)]
        public bool enabled;
        
        [TabGroup(SettingsTabKey)]
        public ViewBindData view;
        
        [TabGroup(SettingsTabKey)]
        public string label;
        
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

        [OnInspectorInit]
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