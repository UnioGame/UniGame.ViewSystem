#if !ODIN_INSPECTOR

using UniGame.UiSystem.Runtime.Settings;
using UniGame.CoreModules.Editor.SerializableTypeEditor;
using UniGame.Core.Runtime.SerializableType.Editor.SerializableTypeEditor;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{
    using UnityEditor;
    using UnityEngine;
    
    [CustomPropertyDrawer(typeof(UiViewReference))]
    public class UiViewReferenceEditor : PropertyDrawer
    {
        private static int _counter = 0;
        private static Color _evenColor = new Color(0.4f, 0.4f, 0.4f);
        private static string _emptyValue = "(empty)";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _counter++;

            // Create property fields.
            var viewProperty = property.FindPropertyRelative("Type");
            var modelTypeProperty = property.FindPropertyRelative("ModelType");
            var viewModelProperty = property.FindPropertyRelative("ViewModelType");

            
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ViewName"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("View"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Tag"));
            
            var sourceModelType = modelTypeProperty.GetSerializedType();
            var viewType = viewProperty.GetSerializedType();
            var viewModelType = viewModelProperty.GetSerializedType();
            var baseViewType = ViewReflectionTool.GetBaseViewType(viewType);
            
            var modelTypeName = sourceModelType == null ? _emptyValue : sourceModelType.Name;
            var viewTypeName = viewType == null ? _emptyValue : viewType.Name;

            EditorGUILayout.Popup( new GUIContent(viewProperty.displayName),0, new[] {viewTypeName});
            EditorGUILayout.Popup( new GUIContent(modelTypeProperty.displayName),0, new[] {modelTypeName});
            TypeDrawer.DrawLayoutTypePopup(new GUIContent(modelTypeProperty.displayName), sourceModelType, viewModelType);


        }
    }
}

#endif