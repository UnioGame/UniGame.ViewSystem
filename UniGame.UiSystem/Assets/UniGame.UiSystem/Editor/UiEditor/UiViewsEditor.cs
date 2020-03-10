using UnityEngine;

namespace Taktika.UI.Editor.UiEditor
{
    using UniGreenModules.UniGame.UiSystem.Runtime.Settings;
    using UnityEditor;

    public class UiViewsEditor : Editor
    {
        private UiAssemblyBuilder builder = new UiAssemblyBuilder();
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("rebuild")) {
                builder.Build(target as UiViewsSource);
            }
        }
    }
}
