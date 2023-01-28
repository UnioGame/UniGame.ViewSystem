using System;
using Cysharp.Threading.Tasks;
using Modules.UniModules.UniGame.ViewSystem.Testing.Editor;
using Sirenix.OdinInspector;
using UniGame.Core.Runtime;
using UniGame.UiSystem.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Modules.UniModules.UniGame.ViewSystem.Testing
{
    public class ViewTestingEnvironment : MonoBehaviour
    {
        #region inspector
        
        [Required]
        [BoxGroup(nameof(settings))]
        public Canvas defaultCanvas;

        [Required]
        [BoxGroup(nameof(settings))]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [HideLabel]
        public ViewTestEnvironmentSettings settings;

        #endregion

        private ViewEditorData _editorSettings = new ViewEditorData();
        private bool _isInitialized = false;
        private GameViewSystemAsset _gameViewSystem;
        
        public bool IsPlaying => Application.isPlaying;

        public bool IsReady => _isInitialized && IsPlaying;

        public bool IsReadyToInitialize => IsPlaying &&
                                           settings != null &&
                                           defaultCanvas != null;
        public ILifeTime LifeTime => this.GetLifeTime();
        
#if UNITY_EDITOR
        [GUIColor(0.1f,0.9f,0.2f)]
        [Button(ButtonSizes.Large)]
        [EnableIf(nameof(IsReady))]
        public void OpenWindow()
        {
            _editorSettings = new ViewEditorData()
            {
                canvas = defaultCanvas,
                settings = settings,
                viewSystem = _gameViewSystem
            };
            
            ViewTestWindow.OpenWindow(_editorSettings);
        }
#endif

        private void Start()
        {
            Initialize();
        }

        [Button(ButtonSizes.Large)]
        [EnableIf(nameof(IsReadyToInitialize))]
        private void Initialize()
        {
            if (!IsReadyToInitialize) return;
            if (_isInitialized) return;
            
            InitializeAsync().Forget();
        }
        
        private async UniTask InitializeAsync()
        {
            var viewSystem = settings.viewSystem;
            var viewSystemObject = Instantiate(viewSystem.gameObject, transform);
            _gameViewSystem = viewSystemObject.GetComponent<GameViewSystemAsset>();
            _gameViewSystem.AddTo(LifeTime);
            
            _isInitialized = true;
        }
    }
}
