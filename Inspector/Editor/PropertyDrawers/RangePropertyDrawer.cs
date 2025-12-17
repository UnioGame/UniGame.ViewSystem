namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Inspector;
    using UnityEditor.UIElements;

    /// <summary>
    /// Property drawer for InspectorRangeAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(InspectorRangeAttribute))]
    public class RangePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var range = attribute as InspectorRangeAttribute;
            var container = new VisualElement();

            if (property.propertyType == SerializedPropertyType.Float)
            {
                var slider = new Slider(range.Min, range.Max)
                {
                    label = property.displayName,
                    value = property.floatValue
                };
                slider.BindProperty(property);
                container.Add(slider);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                var intSlider = new SliderInt((int)range.Min, (int)range.Max)
                {
                    label = property.displayName,
                    value = property.intValue
                };
                intSlider.BindProperty(property);
                container.Add(intSlider);
            }

            return container;
        }
    }
}
