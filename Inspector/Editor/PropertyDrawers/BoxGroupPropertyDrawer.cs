namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Inspector;
    using UnityEditor.UIElements;

    /// <summary>
    /// Property drawer for BoxGroupAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(BoxGroupAttribute))]
    public class BoxGroupPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var boxGroup = attribute as BoxGroupAttribute;
            var container = new VisualElement();
            
            // Create a box-styled container
            container.AddToClassList("box-group");
            container.style.borderLeftColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            container.style.borderLeftWidth = 3;
            container.style.marginLeft = 5;
            container.style.marginRight = 5;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;
            container.style.paddingTop = 5;
            container.style.paddingBottom = 5;

            var field = new PropertyField(property, property.displayName);
            container.Add(field);

            return container;
        }
    }
}
