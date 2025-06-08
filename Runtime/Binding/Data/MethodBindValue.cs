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
    public class MethodBindValue
    {
        private const string SettingsTabKey = "Settings";
        private const string FieldTabKey = "Field";
        
        private static BindingFlags MethodBindingFlags = BindingFlags.Instance | BindingFlags.Public |
                                                         BindingFlags.InvokeMethod |
                                                         BindingFlags.IgnoreCase;
        
        private static BindingFlags FieldBindingFlags = BindingFlags.Instance | BindingFlags.Public |
                                                        BindingFlags.NonPublic | BindingFlags.IgnoreCase;

        private static Type ObservableType = typeof(IObservable<>);
        
#if ODIN_INSPECTOR
        [TabGroup(FieldTabKey)]
        [EnableIf(nameof(enabled))]
        [ValueDropdown(nameof(GetFields))]
        [OnValueChanged(nameof(OnFieldChanged))]
        [HideLabel]
#endif
        public string field = string.Empty;
        
#if ODIN_INSPECTOR
        [TabGroup(FieldTabKey)]
        [ValueDropdown(nameof(GetMethods))]
        [HideLabel]
        [EnableIf(nameof(enabled))]
#endif
        public string method;
        
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
        public IEnumerable<ValueDropdownItem<string>> GetMethods()
        {
            if(view == null) yield break;
            var viewItem = this.view.View;
            if(viewItem == null) yield break;
            
            if (string.IsNullOrEmpty(field))
            {
                yield return new ValueDropdownItem<string>()
                {
                    Text = "select field value",
                    Value = string.Empty,
                };
                yield break;
            }
            
            var modelType = view.ModelType;
            var fieldInfo = modelType.GetField(field,FieldBindingFlags);
            if (fieldInfo == null) yield break;
            
            var viewType = viewItem.GetType();
            var viewMethods = viewType.GetMethods(MethodBindingFlags);
            
            foreach (var methodInfo in viewMethods)
            {
                if(methodInfo.IsGenericMethod || methodInfo.IsSpecialName) continue;
                
                var parameters = methodInfo.GetParameters();
                if(parameters.Length > 1) continue;
                
                if (parameters.Length == 0)
                {
                    yield return new ValueDropdownItem<string>()
                    {
                        Text = methodInfo.Name,
                        Value = methodInfo.Name,
                    };
                    continue;
                }

                if (parameters.Length == 1)
                {
                    var parameter = parameters[0];
                    var targetType = ObservableType.MakeGenericType(parameter.ParameterType);
                    if (targetType.IsAssignableFrom(fieldInfo.FieldType))
                    {
                        yield return new ValueDropdownItem<string>()
                        {
                            Text = methodInfo.Name,
                            Value = methodInfo.Name,
                        };
                    }
                    continue;
                }
                
            }
            
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
            return $"{fieldInfo.Name} : {fieldInfo.FieldType.GetFormattedName()}";
        }
        
    }
}