namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using Inspector;

    /// <summary>
    /// Property drawer for InlineEditorAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(InlineEditorAttribute))]
    public class InlineEditorPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var inlineEditor = attribute as InlineEditorAttribute;
            var container = new VisualElement();
            
            if (inlineEditor.DrawHeader)
            {
                var header = new Label(property.displayName)
                {
                    style =
                    {
                        marginBottom = 5,
                        fontSize = 12,
                        unityFontStyleAndWeight = UnityEngine.FontStyle.Bold
                    }
                };
                container.Add(header);
            }
            
            var field = new PropertyField(property, "");
            field.BindProperty(property);
            container.Add(field);
            
            return container;
        }
    }
}
