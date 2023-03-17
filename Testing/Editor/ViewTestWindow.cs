using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniModules.UniGame.UiSystem.Runtime;
using UniModules.UniGame.ViewSystem;
using UnityEditor;

namespace Modules.UniModules.UniGame.ViewSystem.Testing.Editor
{
    using Sirenix.OdinInspector.Editor;

    public class ViewTestWindow : OdinEditorWindow
    {
        #region static data
        
        public static ViewTestWindow OpenWindow(ViewEditorData settings)
        {
            var window = GetWindow<ViewTestWindow>();
            window.Initialize(settings);
            return window;
        }
        
        #endregion

        private ViewEditorData _settings;

        [HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden,Expanded = true)]
        public ViewTestingEditor viewEditor;
        
        public void Initialize(ViewEditorData editorData)
        {
            _settings = editorData;
            Reload();
        }

        [Button]
        public void Reload()
        {
            viewEditor = CreateInstance<ViewTestingEditor>();
            viewEditor.Initialize(_settings);
        }
    }
}
