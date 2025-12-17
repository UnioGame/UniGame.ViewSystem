namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using Inspector;

    /// <summary>
    /// Property drawer for LabelWidthAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(LabelWidthAttribute))]
    public class LabelWidthPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var labelWidth = attribute as LabelWidthAttribute;
            var field = new PropertyField(property, property.displayName);
            
            field.style.minHeight = 20;
            
            return field;
        }
    }
}
