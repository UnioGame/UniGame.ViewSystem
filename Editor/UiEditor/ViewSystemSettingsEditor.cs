using UniGame.UiSystem.Runtime.Settings;
using UniModules.UniGame.CoreModules.UniGame.Core.Editor.UiElements;
using UnityEditor;
using UnityEngine.UIElements;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{
#if !ODIN_INSPECTOR
    
    [CustomEditor(typeof(ViewSystemSettings),true)]
    public class ViewSystemSettingsEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            serializedObject.Update();
            
            var settings = target as ViewSystemSettings;
            var container = new VisualElement();
 
            // Draw the legacy IMGUI base
            // Create property fields.
            var view = serializedObject.DrawAllChildren(false);

            // Add fields to the container.
            container.Add(view);
            
            return container;
 
        }
    }
    
#endif

}
