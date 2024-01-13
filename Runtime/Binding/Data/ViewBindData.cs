    namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Collections.Generic;
    using Core.Runtime.SerializableType;
    using UiSystem.Runtime;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UnityEngine;

#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
#if ODIN_INSPECTOR
     using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public class ViewBindData : MonoBehaviour
    {
        private const string SettingsTabKey = "Settings";
        private const string BindTabKey = "Bind";
        private static Type BaseModelType = typeof(IViewModel);

        public bool isEnabled = true;
        
#if ODIN_INSPECTOR
        [OnValueChanged(nameof(Save))]
        [TabGroup(BindTabKey)]
        [ListDrawerSettings(CustomAddFunction = nameof(AddField),ListElementLabelName = "label")]
#endif
        public List<FieldBindValue> values = new List<FieldBindValue>();
        
#if ODIN_INSPECTOR
        [PropertySpace(8)]
        [OnValueChanged(nameof(Save))]
        [TabGroup(BindTabKey)]
        [ListDrawerSettings(CustomAddFunction = nameof(AddMethod),ListElementLabelName = "label")]
#endif
        public List<MethodBindValue> methods = new List<MethodBindValue>();
        
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
#endif
        public bool autoBindModel = true;
        
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
        [HideIf(nameof(autoBindModel))]
        [ValueDropdown(nameof(GetModelTypes))]
#endif
        public SType modelType;

#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
#endif
        public ViewBase view;

        public Type ModelType => autoBindModel ? view.ModelType : modelType;
        
        public IView View => view??= GetComponent<ViewBase>();
        
        public void AddField()
        {
            var value = new FieldBindValue()
            {
                view = this,
                enabled = true,
            };
            values.Add(value);
            Save();
        }
        
        public void AddMethod()
        {
            var value = new MethodBindValue()
            {
                view = this,
                enabled = true,
            };
            methods.Add(value);
            Save();
        }

        public void Save()
        {
#if UNITY_EDITOR
            this.MarkDirty();
            gameObject.MarkDirty();
#endif
        }
        
#if ODIN_INSPECTOR
        public IEnumerable<ValueDropdownItem<SType>> GetModelTypes()
        {
            var types = BaseModelType.GetAssignableTypes();
            foreach (var type in types)
            {
                var value = (SType)type;

                yield return new ValueDropdownItem<SType>()
                {
                    Text = type.Name,
                    Value = value,
                };
            }
        }
#endif
        
#if ODIN_INSPECTOR
        [OnInspectorInit]
#endif
        private void OnInspectorInit()
        {
            view ??= GetComponent<ViewBase>();
        }

    }
}