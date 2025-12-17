namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEngine.UIElements;
    using Inspector;
    using UnityEditor.UIElements;

    /// <summary>
    /// Property drawer for TitleAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitlePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var title = attribute as TitleAttribute;
            var container = new VisualElement();

            // Add title label
            var titleLabel = new Label(title.Title);
            titleLabel.AddToClassList("inspector-title");
            titleLabel.style.fontSize = 14;
            titleLabel.style.marginBottom = 5;
            titleLabel.style.marginTop = 10;
            container.Add(titleLabel);

            // Add subtitle if present
            if (!string.IsNullOrEmpty(title.Subtitle))
            {
                var subtitleLabel = new Label(title.Subtitle);
                subtitleLabel.AddToClassList("inspector-subtitle");
                subtitleLabel.style.fontSize = 11;
                subtitleLabel.style.marginBottom = 10;
                container.Add(subtitleLabel);
            }

            // Add the property field
            var field = new PropertyField(property, property.displayName);
            container.Add(field);

            return container;
        }
    }
}
