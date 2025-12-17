namespace UniGame.ViewSystem.Inspector.Editor.Utilities
{
    using UnityEditor;
    using UnityEngine.UIElements;
    using Inspector;
    using UnityEngine;

    /// <summary>
    /// Helper class for managing inspector UI with UI Toolkit
    /// </summary>
    public static class InspectorUIHelper
    {
        /// <summary>
        /// Creates a visual element container for a group
        /// </summary>
        public static VisualElement CreateGroupContainer(string groupName, bool showToggle = true, bool expanded = true)
        {
            var container = new VisualElement();
            container.AddToClassList("box-group-container");
            
            if (showToggle)
            {
                var foldout = new Foldout { text = groupName, value = expanded };
                foldout.AddToClassList("box-group-foldout");
                container.Add(foldout);
            }
            else
            {
                var label = new Label(groupName);
                label.AddToClassList("box-group-title");
                container.Add(label);
            }

            return container;
        }

        /// <summary>
        /// Creates a title element
        /// </summary>
        public static Label CreateTitleElement(string title, string subtitle = "", string color = "white")
        {
            var label = new Label();
            label.text = string.IsNullOrEmpty(subtitle) ? title : $"{title}\n{subtitle}";
            label.AddToClassList("inspector-title");
            label.style.color = ParseColor(color);
            return label;
        }

        /// <summary>
        /// Creates a horizontal layout for fields
        /// </summary>
        public static VisualElement CreateHorizontalGroup()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.AddToClassList("horizontal-group");
            return container;
        }

        /// <summary>
        /// Parses color from string (hex or color name)
        /// </summary>
        private static Color ParseColor(string colorString)
        {
            if (ColorUtility.TryParseHtmlString(colorString, out Color color))
                return color;

            // Try standard color names
            return colorString.ToLower() switch
            {
                "white" => Color.white,
                "black" => Color.black,
                "red" => Color.red,
                "green" => Color.green,
                "blue" => Color.blue,
                "yellow" => Color.yellow,
                "cyan" => Color.cyan,
                "magenta" => Color.magenta,
                "gray" => Color.gray,
                _ => Color.white
            };
        }
    }
}
