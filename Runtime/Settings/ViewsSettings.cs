#if UNITY_EDITOR
using UniModules.UniGame.AddressableExtensions.Editor;
#endif

using UniModules.UniGame.Core.Runtime.ScriptableObjects;

namespace UniGame.UiSystem.Runtime.Settings
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/ViewSystem/UiViewsSettings", fileName = "UiViewsSettings")]
    public class ViewsSettings : LifetimeScriptableObject, IViewsSettings
    {
        public string sourceName;

#if ODIN_INSPECTOR
#if UNITY_EDITOR
        [Sirenix.OdinInspector.ValueDropdown(nameof(GetLabels),ExcludeExistingValuesInList = true)]
#endif
#endif
        public List<string> labels = new List<string>();

        [Header("Is Source will be filled on Update")]
        public bool isActive = true;

        [HideInInspector] [Header("Name of target addressable group")]
        public string addressableGroupName;

        [Space]
        [Header("Default views")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FolderPath]
#endif
        public List<string> uiViewsDefaultFolders = new List<string>();

        [Header("Skins folder")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FolderPath]
#endif
        public List<string> uiViewsSkinFolders = new List<string>();

        [Header("Registered Views")] [Space] public List<UiViewReference> uiViews = new List<UiViewReference>();

        public bool IsActive => isActive;

        public IReadOnlyList<UiViewReference> Views => uiViews;

#if UNITY_EDITOR
        private IEnumerable<string> GetLabels()
        {
            return this.GetAllAddressablesLabels();
        }
#endif
    }
}