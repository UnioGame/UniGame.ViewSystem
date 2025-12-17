namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Inspector;

    /// <summary>
    /// Property drawer for TabGroupAttribute - displays fields in a visual tab group
    /// </summary>
    [CustomPropertyDrawer(typeof(TabGroupAttribute))]
    public class TabGroupPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tabGroup = attribute as TabGroupAttribute;
            var container = new VisualElement();
            
            // Create a visual tab indicator
            var tabHeader = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f),
                    paddingLeft = 5,
                    paddingRight = 5,
                    paddingTop = 2,
                    paddingBottom = 2,
                    marginBottom = 5,
                    borderBottomColor = new Color(0.2f, 0.2f, 0.2f, 1f),
                    borderBottomWidth = 2
                }
            };

            var groupLabel = new Label(tabGroup.GroupName)
            {
                style =
                {
                    fontSize = 10,
                    color = new Color(0.7f, 0.7f, 0.7f, 1f),
                    marginRight = 5
                }
            };

            var tabLabel = new Label(tabGroup.TabName)
            {
                style =
                {
                    fontSize = 11,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = Color.white,
                    paddingLeft = 5,
                    paddingRight = 5,
                    paddingTop = 2,
                    paddingBottom = 2,
                    backgroundColor = new Color(0.2f, 0.4f, 0.6f, 1f),
                    borderTopLeftRadius = 3,
                    borderTopRightRadius = 3
                }
            };

            tabHeader.Add(groupLabel);
            tabHeader.Add(tabLabel);
            container.Add(tabHeader);
            
            var field = new PropertyField(property, property.displayName);
            field.BindProperty(property);
            container.Add(field);
            
            return container;
        }
    }
}

