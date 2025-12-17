namespace UniGame.ViewSystem.Inspector.Editor.Editors
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using System.Reflection;
    using System.Linq;

    /// <summary>
    /// Generic editor for all MonoBehaviour objects with ButtonAttribute methods
    /// Automatically renders buttons and other custom inspector elements via UI Toolkit
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), isFallback = true)]
    public class GenericAttributeInspectorEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            // Draw default inspector
            var defaultInspector = new IMGUIContainer(() =>
            {
                DrawDefaultInspector();
            });
            root.Add(defaultInspector);

            // Draw buttons if any exist
            var targetType = target.GetType();
            var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var buttonMethods = methods.Where(m => m.GetCustomAttribute(typeof(ButtonAttribute)) != null).ToList();

            if (buttonMethods.Count > 0)
            {
                var actionsContainer = new VisualElement()
                {
                    style =
                    {
                        marginTop = 15,
                        paddingLeft = 5,
                        paddingRight = 5,
                        paddingTop = 10,
                        paddingBottom = 10,
                        borderTopColor = new Color(0.3f, 0.3f, 0.3f, 1f),
                        borderTopWidth = 2
                    }
                };

                var actionsLabel = new Label("Actions")
                {
                    style =
                    {
                        fontSize = 14,
                        unityFontStyleAndWeight = FontStyle.Bold,
                        marginBottom = 10,
                        marginTop = 5
                    }
                };
                actionsContainer.Add(actionsLabel);

                foreach (var method in buttonMethods)
                {
                    var buttonAttr = method.GetCustomAttribute(typeof(ButtonAttribute)) as ButtonAttribute;
                    if (buttonAttr != null)
                    {
                        var buttonLabel = string.IsNullOrEmpty(buttonAttr.Label) ? method.Name : buttonAttr.Label;
                        var height = buttonAttr.Height > 0 ? buttonAttr.Height : 30;

                        var button = new Button(() =>
                        {
                            method.Invoke(target, null);
                        })
                        {
                            text = buttonLabel,
                            style =
                            {
                                height = height,
                                marginBottom = 8,
                                fontSize = 12,
                                paddingLeft = 10,
                                paddingRight = 10,
                                paddingTop = 5,
                                paddingBottom = 5,
                                unityFontStyleAndWeight = FontStyle.Bold
                            }
                        };

                        actionsContainer.Add(button);
                    }
                }

                root.Add(actionsContainer);
            }

            return root;
        }
    }
}
