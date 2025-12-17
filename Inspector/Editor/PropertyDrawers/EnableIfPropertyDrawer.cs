namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Inspector;

    /// <summary>
    /// Property drawer for EnableIfAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    public class EnableIfPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var enableIf = attribute as EnableIfAttribute;
            var field = new PropertyField(property, property.displayName);
            field.BindProperty(property);
            
            UpdateFieldState(property, enableIf, field);
            
            return field;
        }

        private void UpdateFieldState(SerializedProperty property, EnableIfAttribute enableIf, PropertyField field)
        {
            var conditionProperty = property.serializedObject.FindProperty(enableIf.ConditionField);
            if (conditionProperty != null)
            {
                bool shouldEnable = GetConditionValue(conditionProperty, enableIf.ExpectedValue);
                field.SetEnabled(shouldEnable);
            }
        }

        private bool GetConditionValue(SerializedProperty property, object expectedValue)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.Boolean => property.boolValue == (bool)expectedValue,
                SerializedPropertyType.Integer => property.intValue == (int)expectedValue,
                SerializedPropertyType.String => property.stringValue == (string)expectedValue,
                _ => false
            };
        }
    }
}
