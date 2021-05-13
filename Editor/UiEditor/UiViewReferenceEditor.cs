

#if !ODIN_INSPECTOR && ENABLE_UI_TOOLKIT

using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{
    using System.Collections.Generic;
    using UniGame.UiSystem.Runtime.Settings;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using UniModules.UniGame.CoreModules.UniGame.Core.Editor.SerializableTypeEditor;
    using UniModules.UniGame.CoreModules.UniGame.Core.Editor.UiElements;

    [CustomPropertyDrawer(typeof(UiViewReference))]
    public class UiViewReferenceEditor : PropertyDrawer
    {
        private static int _counter = 0;
        private static Color _evenColor = new Color(0.4f, 0.4f, 0.4f);
        private static string _emptyValue = "(empty)";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _counter++;
            // Create property container element.
            var container = new VisualElement();
            container.style.marginTop = 4;

            if (_counter % 2 == 0)
            {
                container.style.backgroundColor = _evenColor;
            }

            // Create property fields.
            var viewProperty = property.FindPropertyRelative("Type");
            var modelTypeProperty = property.FindPropertyRelative("ModelType");
            var viewModelProperty = property.FindPropertyRelative("ViewModelType");

            var view = new PropertyField(property.FindPropertyRelative("View"));
            var viewName = new PropertyField(property.FindPropertyRelative("ViewName"));
            var tag = new PropertyField(property.FindPropertyRelative("Tag"));
            

            var sourceModelType = modelTypeProperty.GetSerializedType();
            var viewType = viewProperty.GetSerializedType();
            var baseViewType = ViewReflectionTool.GetBaseViewType(viewType);
            
            // Add fields to the container.
            container.Add(view);
            container.Add(viewName);
            container.Add(tag);
            
            var modelTypeName = sourceModelType == null ? _emptyValue : sourceModelType.Name;
            var modelType = new DropdownField(modelTypeProperty.displayName,new List<string>(){modelTypeName},0);
            var viewDropDown = UiElementsExtensions.CreateSerializedTypeDropDown(baseViewType, viewProperty);
            var viewModelDropDown = UiElementsExtensions.CreateSerializedTypeDropDown(sourceModelType, viewModelProperty);
   
            container.Add(viewDropDown);
            container.Add(modelType);
            container.Add(viewModelDropDown);
        
            container.Add(new PropertyField(viewProperty));
            container.Add(new PropertyField(modelTypeProperty));
            container.Add(new PropertyField(viewModelProperty));

            return container;

        }
    }
}


#endif