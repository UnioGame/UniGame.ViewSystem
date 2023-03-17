using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniGame.Core.Runtime.SerializableType;
using UniGame.Core.Runtime.SerializableType.Attributes;
using UniGame.ViewSystem.Runtime;
using UniModules.UniGame.ViewSystem;
using UnityEngine;

namespace Modules.UniModules.UniGame.ViewSystem.Testing
{
    [Serializable]
    public class ViewTestingData : ISearchFilterable
    {
        private const string ViewInfoGroup = "View Info";

        #region inspector
        
        public string name = string.Empty;
        
        /// <summary>
        /// type of target view
        /// </summary>
        [BoxGroup(ViewInfoGroup)]
        [OnValueChanged(nameof(OnViewTypeChanged))]
        [DrawWithUnity]
        [STypeFilter(typeof(IView))]
        public SType viewType;

        [BoxGroup(ViewInfoGroup)]
        [ValueDropdown(nameof(GetModelVariants))]
        public SType viewModelType;

        [BoxGroup(ViewInfoGroup)]
        public string skin = string.Empty;
        
        [BoxGroup(nameof(Provider))]
        [SerializeReference]
        [HideIf(nameof(IsAssetProvider))]
        [InlineProperty]
        public ITestViewModelProvider serializableProvider;
        
        [BoxGroup(nameof(Provider))]
        [HideIf(nameof(IsSerializableProvider))]
        [InlineEditor()]
        public TestViewModelAsset assetProvider;

        #endregion
        
        public ITestViewModelProvider Provider => IsAssetProvider 
            ? assetProvider : serializableProvider;
        
        public bool IsAssetProvider => assetProvider != null;
        
        public bool IsSerializableProvider => serializableProvider != null;
        
        public IEnumerable<ValueDropdownItem<SType>> GetModelVariants()
        {
            var modelType = GetViewModelType();
            if (modelType == null)
            {
                var type = typeof(EmptyViewModel);
                yield return new ValueDropdownItem<SType>(type.Name, type);
                yield break;
            }

            var items = ViewSystemUtils.GetModelSTypeVariants(modelType);
            foreach (var variant in items)
                yield return variant;
        }

        private Type GetViewModelType()
        {
            var type = viewType.type;
            if (type == null) return typeof(EmptyViewModel);
            var modelType = ViewSystemUtils.GetModelTypeByView(type);
            return modelType;
        }

        [OnInspectorInit]
        private void OnInspectorInitialize()
        {
            name = viewType.type == null ? string.Empty : viewType.type.Name;
            viewModelType = viewModelType.type ?? GetViewModelType();
        }

        private void OnViewTypeChanged()
        {
            if (viewType.type == null)
            {
                viewModelType = typeof(EmptyViewModel);
                return;
            }
            
            viewModelType = GetViewModelType();
        }

        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;

            if (name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (viewType.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
            if (viewModelType.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
            if (skin.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (Provider != null && Provider.GetType().Name
                    .Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}