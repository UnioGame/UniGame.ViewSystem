namespace UniGame.ViewSystem.Inspector.Editor.Utilities
{
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Inspector;

    /// <summary>
    /// Scans types and draws buttons for methods with ButtonAttribute
    /// </summary>
    public class ButtonMethodDrawer
    {
        /// <summary>
        /// Adds button elements to a container for all methods with ButtonAttribute
        /// </summary>
        public static void AddButtonMethodsToContainer(Object targetObject, VisualElement container)
        {
            if (targetObject == null) return;

            MethodInfo[] methods = targetObject.GetType().GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MethodInfo method in methods)
            {
                var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttr == null) continue;

                // Skip methods with parameters
                if (method.GetParameters().Length != 0) continue;

                string label = string.IsNullOrEmpty(buttonAttr.Label) ? method.Name : buttonAttr.Label;

                var button = new Button(() => method.Invoke(targetObject, null))
                {
                    text = label
                };

                button.style.height = buttonAttr.Height;
                button.style.marginTop = 5;
                button.style.marginBottom = 5;

                container.Add(button);
            }
        }
    }
}
