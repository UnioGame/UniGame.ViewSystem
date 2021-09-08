namespace UniGame.UiSystem.Runtime.Settings
{
#if UNITY_EDITOR
    using UniModules.UniGame.AddressableExtensions.Editor;
#endif
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
        private const string ViewsGroup = "views";
        private const string AddressablesGroup = "";
        private static Color ViewsBackgroundColor = new Color(0.7f, 0.9f, 0.9f);
        public string sourceName;

        [Header("Is Source will be filled on Update")]
        public bool isActive = true;

        [Header("Default views")]
#if ODIN_INSPECTOR
        [FolderPath]
        [GUIColor(nameof(ViewsBackgroundColor))]
        [BoxGroup(nameof(ViewsGroup))]
#endif
        public List<string> uiViewsDefaultFolders = new List<string>();

        [Header("Skins folder")]
#if ODIN_INSPECTOR
        [GUIColor(nameof(ViewsBackgroundColor))]
        [BoxGroup(nameof(ViewsGroup))]
        [FolderPath]
#endif
        public List<string> uiViewsSkinFolders = new List<string>();

        [Header("Registered Views")]
#if ODIN_INSPECTOR_3
        [Searchable]
#endif
#if ODIN_INSPECTOR
        [BoxGroup(nameof(ViewsGroup))]
        [GUIColor(nameof(ViewsBackgroundColor))]
#endif
        public List<UiViewReference> uiViews = new List<UiViewReference>();

        [Space]
#if ODIN_INSPECTOR
        [BoxGroup(nameof(AddressablesGroup))]
#endif
        public bool applyAddressablesGroup = false;

#if ODIN_INSPECTOR
#if UNITY_EDITOR
        [ValueDropdown(nameof(GetLabels), ExcludeExistingValuesInList = true)]
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