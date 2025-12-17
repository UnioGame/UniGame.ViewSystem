namespace UniGame.ViewSystem.Inspector.Editor.Settings
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;

    /// <summary>
    /// Configuration for Inspector UI Toolkit styles loading
    /// Uses relative paths to support package portability
    /// </summary>
    public class InspectorStylesConfig
    {
        /// <summary>
        /// Path to the Inspector package relative to Assets
        /// </summary>
        private const string INSPECTOR_PACKAGE_PATH = "Packages/com.unigame.viewsystem/Inspector";

        /// <summary>
        /// Path to styles folder relative to Inspector package
        /// </summary>
        private const string STYLES_FOLDER_PATH = "Editor/Resources/Styles";

        /// <summary>
        /// Gets the full relative path to styles folder
        /// </summary>
        public static string GetStylesFolderPath() => $"{INSPECTOR_PACKAGE_PATH}/{STYLES_FOLDER_PATH}";

        /// <summary>
        /// Gets the full path to a specific style file
        /// </summary>
        public static string GetStyleFilePath(string styleName) 
            => $"{GetStylesFolderPath()}/{styleName}.uss";

        /// <summary>
        /// Loads a style sheet from the Inspector package
        /// </summary>
        public static StyleSheet LoadInspectorStyle(string styleName)
        {
            string path = GetStyleFilePath(styleName);
            return Resources.Load<StyleSheet>(path);
        }

        /// <summary>
        /// Main inspector styles
        /// </summary>
        public static StyleSheet GetInspectorStyles()
        {
            return LoadInspectorStyle("InspectorStyles");
        }

        /// <summary>
        /// Applies inspector styles to a visual element
        /// </summary>
        public static void ApplyInspectorStyles(VisualElement element)
        {
            var style = GetInspectorStyles();
            if (style != null)
            {
                element.styleSheets.Add(style);
            }
        }
    }
}
