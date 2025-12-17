namespace UniGame.ViewSystem.Inspector.Editor.Utilities
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using Inspector;

    /// <summary>
    /// Handles rendering of custom inspector attributes in the inspector
    /// Manages buttons, tabs, groups, and other layout elements
    /// </summary>
    public static class AttributeInspectorRenderer
    {
        public static void RenderInspector(SerializedObject serializedObject)
        {
            var targetObject = serializedObject.targetObject;
            var targetType = targetObject.GetType();

            // Draw button methods
            RenderButtons(targetObject, targetType);
        }

        private static void RenderButtons(object targetObject, Type targetType)
        {
            var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var method in methods)
            {
                var buttonAttr = method.GetCustomAttribute(typeof(ButtonAttribute)) as ButtonAttribute;
                if (buttonAttr != null)
                {
                    var label = buttonAttr.Label ?? method.Name;
                    if (GUILayout.Button(label, GUILayout.Height(buttonAttr.Height)))
                    {
                        method.Invoke(targetObject, null);
                    }
                }
            }
        }
    }
}
