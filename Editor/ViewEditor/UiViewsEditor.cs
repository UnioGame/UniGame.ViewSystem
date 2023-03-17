using UniGame.UiSystem.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.ViewSystem
{
#if !ODIN_INSPECTOR
    [CustomEditor(typeof(ViewsSettings),editorForChildClasses:true)]
#endif
    public class UiViewsEditor : UnityEditor.Editor
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
