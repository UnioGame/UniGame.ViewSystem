namespace UniGame.ViewSystem.Inspector.Editor.PropertyDrawers
{
    using UnityEditor;
    using UnityEngine;
    using Inspector;

    /// <summary>
    /// Property drawer for PreviewAttribute - displays texture/sprite previews
    /// </summary>
    [CustomPropertyDrawer(typeof(PreviewAttribute))]
    public class PreviewPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var preview = attribute as PreviewAttribute;
            
            // Base property height + preview box height + spacing
            float baseHeight = EditorGUI.GetPropertyHeight(property, label);
            float previewHeight = 0;

            if (property.objectReferenceValue != null && 
                (property.objectReferenceValue is Texture2D || property.objectReferenceValue is Sprite))
            {
                previewHeight = preview.Height + 10; // Preview height + spacing
            }

            return baseHeight + previewHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var preview = attribute as PreviewAttribute;

            // Draw the property field
            Rect propertyRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(property, label));
            EditorGUI.PropertyField(propertyRect, property, label);

            // Draw preview if object is assigned
            if (property.objectReferenceValue != null)
            {
                Object obj = property.objectReferenceValue;
                Texture2D texture = null;

                // Get texture from Sprite or direct Texture2D reference
                if (obj is Sprite sprite)
                {
                    texture = sprite.texture;
                }
                else if (obj is Texture2D tex)
                {
                    texture = tex;
                }

                if (texture != null)
                {
                    Rect previewRect = new Rect(
                        position.x + EditorGUIUtility.labelWidth,
                        position.y + EditorGUI.GetPropertyHeight(property, label) + 5,
                        preview.Width,
                        preview.Height
                    );

                    // Draw border
                    EditorGUI.DrawRect(
                        new Rect(previewRect.x - 1, previewRect.y - 1, previewRect.width + 2, previewRect.height + 2),
                        new Color(0.5f, 0.5f, 0.5f, 1f)
                    );

                    // Draw preview
                    GUI.DrawTexture(previewRect, texture, ScaleMode.ScaleToFit, alphaBlend: true);
                }
            }
        }
    }
}
