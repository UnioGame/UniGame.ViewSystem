namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Inspector;
    using UnityEditor.UIElements;
    using Utilities;

    /// <summary>
    /// Property drawer for ShowIfAttribute and HideIfAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class ConditionalPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            var showIf = attribute as ShowIfAttribute;
            var hideIf = attribute as HideIfAttribute;

            var field = new PropertyField(property, property.displayName);

            // Determine visibility
            bool shouldShow = true;
            object target = PropertyDrawerUtility.GetTargetObjectOfProperty(property);

            if (showIf != null)
            {
                if (PropertyDrawerUtility.TryGetConditionValue(target, showIf.ConditionName, out bool result))
                {
                    shouldShow = showIf.Invert ? !result : result;
                }
            }
            else if (hideIf != null)
            {
                if (PropertyDrawerUtility.TryGetConditionValue(target, hideIf.ConditionName, out bool result))
                {
                    shouldShow = hideIf.Invert ? result : !result;
                }
            }

            field.style.display = shouldShow ? DisplayStyle.Flex : DisplayStyle.None;
            container.Add(field);

            return container;
        }
    }
}
