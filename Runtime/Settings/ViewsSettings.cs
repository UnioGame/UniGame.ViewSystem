#if UNITY_EDITOR
using UniModules.UniGame.AddressableExtensions.Editor;
#endif

namespace UniGame.UiSystem.Runtime.Settings
{
    using UniModules.UniGame.Core.Runtime.ScriptableObjects;
    using UniRx;
    using System.Collections.Generic;
    using UnityEngine;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    [CreateAssetMenu(menuName = "UniGame/ViewSystem/UiViewsSettings", fileName = "ViewsSettings")]
    public class ViewsSettings : LifetimeScriptableObject, IViewsSettings
    {
        private const string AddressablesGroup = "";
        public string sourceName;

        [Header("Is Source will be filled on Update")]
        public bool isActive = true;

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


        [Space]
#if ODIN_INSPECTOR
        [BoxGroup(nameof(AddressablesGroup))]
#endif
        public bool applyAddressablesGroup = false;

#if ODIN_INSPECTOR
#if UNITY_EDITOR
        [Sirenix.OdinInspector.ValueDropdown(nameof(GetLabels), ExcludeExistingValuesInList = true)]
#endif
        [ShowIf(nameof(applyAddressablesGroup))]
        [BoxGroup(nameof(AddressablesGroup))]
#endif
        public List<string> labels = new List<string>();

#if ODIN_INSPECTOR
        [BoxGroup(nameof(AddressablesGroup))]
        [ShowIf(nameof(applyAddressablesGroup))]
#endif
        [Header("Name of target addressable group")]
        public string addressableGroupName;


        [Header("Registered Views")]
        [Space]
#if ODIN_INSPECTOR_3
        [Sirenix.OdinInspector.Searchable]
#endif
        public List<UiViewReference> uiViews = new List<UiViewReference>();

        public bool IsActive => isActive;

        public IReadOnlyList<UiViewReference> Views => uiViews;

#if UNITY_EDITOR
        private IEnumerable<string> GetLabels()
        {
            return this.GetAllAddressablesLabels();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        private void Rebuild()
        {
            MessageBroker.Default.Publish(new SettingsRebuildMessage() {ViewsSettings = this});
        }

#endif
    }
}