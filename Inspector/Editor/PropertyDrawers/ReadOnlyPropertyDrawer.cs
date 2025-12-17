namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEngine.UIElements;
    using Inspector;
    using UnityEditor.UIElements;

    /// <summary>
    /// Property drawer for ReadOnlyAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var readOnly = attribute as ReadOnlyAttribute;
            var container = new VisualElement();

            var field = new PropertyField(property, property.displayName);
            
            if (readOnly.ShowDisabled)
            {
                field.SetEnabled(false);
            }

            container.Add(field);
            return container;
        }
    }
}
