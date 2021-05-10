
namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{
    using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;
    using UnityEditor;
    using UnityEngine;
    
#if !ODIN_INSPECTOR
    [CustomEditor(typeof(ViewModelProviderSettings),editorForChildClasses:true)]
#endif
    public class ViewContextMapSettingsEditor : UnityEditor.Editor
    {
        private ViewsAssemblyBuilder builder = new ViewsAssemblyBuilder();
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("rebuild")) {
                builder.RebuildAll();
            }
        }
    }
}