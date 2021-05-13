#if !ODIN_INSPECTOR && ENABLE_UI_TOOLKIT

using UniGame.UiSystem.Runtime.Settings;
using UnityEditor;
using UnityEngine.UIElements;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{

    using UniModules.UniGame.CoreModules.UniGame.Core.Editor.UiElements;

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

}


#endif
