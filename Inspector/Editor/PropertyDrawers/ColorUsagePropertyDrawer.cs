namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Inspector;
    using UnityEditor.UIElements;

    /// <summary>
    /// Property drawer for InspectorColorUsageAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(InspectorColorUsageAttribute))]
    public class ColorUsagePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var colorUsage = attribute as InspectorColorUsageAttribute;
            
            if (property.propertyType == SerializedPropertyType.Color)
            {
                var colorField = new ColorField(property.displayName)
                {
                    showAlpha = colorUsage.ShowAlpha,
                };
                colorField.BindProperty(property);
                
                return colorField;
            }
            
            return new PropertyField(property, property.displayName);
        }
    }
}
