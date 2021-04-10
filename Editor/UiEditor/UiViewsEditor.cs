using UnityEngine;

namespace UniGame.UiSystem.UI.Editor.UiEditor
{
    using Runtime.Settings;
    using UniGame.UiSystem.Editor.UiEditor;
    using UnityEditor;

    public class UiViewsEditor : Editor
    {
        private ViewsAssemblyBuilder builder = new ViewsAssemblyBuilder();
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("rebuild")) {
                builder.Build(target as ViewsSettings);
            }
        }
    }
}
