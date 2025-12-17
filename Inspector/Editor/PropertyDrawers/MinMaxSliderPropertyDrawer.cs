namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Inspector;

    /// <summary>
    /// Property drawer for MinMaxSliderAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var minMaxSlider = attribute as MinMaxSliderAttribute;
            var container = new VisualElement();

            // Only works with Vector2
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                var warning = new Label("MinMaxSlider only works with Vector2 fields");
                warning.style.color = Color.red;
                container.Add(warning);
                return container;
            }

            var vector = property.vector2Value;
            var slider = new MinMaxSlider(property.displayName, vector.x, vector.y, minMaxSlider.Min, minMaxSlider.Max);
            
            slider.RegisterValueChangedCallback(evt =>
            {
                property.vector2Value = new Vector2(evt.newValue.x, evt.newValue.y);
                property.serializedObject.ApplyModifiedProperties();
            });

            container.Add(slider);
            return container;
        }
    }
}
