namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using Inspector;

    /// <summary>
    /// Property drawer for FoldoutGroupAttribute
    /// </summary>
    [CustomPropertyDrawer(typeof(FoldoutGroupAttribute))]
    public class FoldoutGroupPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var foldoutGroup = attribute as FoldoutGroupAttribute;
            var container = new VisualElement();
            
            var foldout = new Foldout
            {
                text = foldoutGroup.GroupName,
                value = foldoutGroup.ExpandedByDefault
            };
            
            var field = new PropertyField(property, property.displayName);
            field.BindProperty(property);
            foldout.Add(field);
            container.Add(foldout);
            
            return container;
        }
    }
}
