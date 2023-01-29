using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniGame.Core.Runtime.SerializableType;
using UniGame.Core.Runtime.SerializableType.Attributes;
using UniGame.UiSystem.Runtime.Settings;
using UniGame.ViewSystem.Runtime;
using UniModules.UniCore.Runtime.ReflectionUtils;
using UnityEngine;

namespace Modules.UniModules.UniGame.ViewSystem.Testing.Editor
{
    public class ViewTestingEditor : ScriptableObject
    {
        private const string NotFoundViewMessage = "ViewType not found in target settings";

        #region inspector

        [BoxGroup(nameof(settings))]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [HideLabel]
        public ViewTestEnvironmentSettings settings;

        /// <summary>
        /// type of target view
        /// </summary>
        [ValueDropdown(nameof(GetViewTypes))]
        public SType viewType;

        public bool useOnlyDefinedTypes = true;
        
        public ViewShownType viewShownType = ViewShownType.None;
        
        [InfoBox("info")]
        [Multiline(lines:2)]
        [ReadOnly]
        public string message;
        
        #endregion
        
        private ViewEditorData _editorData;
        private ViewSystemSettings _viewSystemSettings;
        private IView _activeObject;

        public void Initialize(ViewEditorData editorData)
        {
            _editorData = editorData;
            _viewSystemSettings = settings.viewSettings;
            
            settings = editorData.settings;
        }

        [Button]
        public void Show()
        {
            OpenView(viewShownType).Forget();
        }

        [Button]
        public void Close()
        {
            if(_activeObject!=null) _activeObject.Close();
        }

        public async UniTask OpenView(ViewShownType viewShownType)
        {
            if(!Application.isPlaying || this.viewType.type == null) return;

            var viewSystem = _editorData.viewSystem;
            var canvas = _editorData.canvas;
            
            var viewData = settings.viewsData;
            var target = viewData
                .FirstOrDefault(x => x.viewType.Equals(this.viewType));
            
            if (target == null)
            {
                message = NotFoundViewMessage;
                return;
            }

            var provider = target.Provider;
            var viewType = target.viewType;
            var skin = target.skin;
            var viewModel = provider == null ? new EmptyViewModel() : provider.Create();
            
            if(_activeObject != null) _activeObject.Close();
            
            switch (viewShownType)
            {
                case ViewShownType.None:
                    _activeObject = await viewSystem
                        .Create(viewModel, viewType, skin, canvas.transform);
                    _activeObject.Show();
                    break;
                case ViewShownType.Window:
                    _activeObject = await viewSystem.OpenWindow(viewModel, viewType, skin);
                    break;
                case ViewShownType.Screen:
                    _activeObject = await viewSystem.OpenScreen(viewModel, viewType, skin);
                    break;
                case ViewShownType.Overlay:
                    _activeObject = await viewSystem.OpenOverlay(viewModel, viewType, skin);
                    break;
            }
      
        }

        public IEnumerable<ValueDropdownItem<SType>> GetViewTypes()
        { 
            if (useOnlyDefinedTypes)
            {
                foreach (var data in settings.viewsData)
                {
                    var dataType = data.viewType;
                    yield return new ValueDropdownItem<SType>()
                    {
                        Text = dataType.Name,
                        Value = dataType
                    };
                }
                yield break;
            }
            
            var baseType = typeof(IView);
            var types = baseType.GetAssignableTypes();
            foreach (var type in types)
            {
                if(type.IsAbstract || type.IsInterface) continue;
                yield return new ValueDropdownItem<SType>()
                {
                    Text = type.Name,
                    Value = type
                };
            }
        }
    }
}