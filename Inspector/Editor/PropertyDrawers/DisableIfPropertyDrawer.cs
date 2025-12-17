namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Inspector;

    /// <summary>
    /// Property drawer for DisableIfAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var disableIf = attribute as DisableIfAttribute;
            var field = new PropertyField(property, property.displayName);
            field.BindProperty(property);
            
            UpdateFieldState(property, disableIf, field);
            
            return field;
        }

        private void UpdateFieldState(SerializedProperty property, DisableIfAttribute disableIf, PropertyField field)
        {
            var conditionProperty = property.serializedObject.FindProperty(disableIf.ConditionField);
            if (conditionProperty != null)
            {
                bool shouldDisable = GetConditionValue(conditionProperty, disableIf.ExpectedValue);
                field.SetEnabled(!shouldDisable);
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
