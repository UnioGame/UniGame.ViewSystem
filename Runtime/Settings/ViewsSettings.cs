namespace UniGame.UiSystem.Runtime.Settings
{
#if UNITY_EDITOR
    using AddressableTools.Editor;
#endif
     
    using System.Collections.Generic;
    using UniGame.Runtime.Rx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    [CreateAssetMenu(menuName = "UniGame/ViewSystem/" + nameof(ViewsSettings), fileName = nameof(ViewsSettings))]
    public class ViewsSettings : ScriptableObject, IViewsSettings
    {
        public const string ViewTabKey = "Views";
        public const string SettingsTabKey = "Settings";

        private static Color ViewsBackgroundColor = new(0.7f, 0.9f, 0.9f);
        
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        public string sourceName;

        [Header("Is Source will be filled on Update")]
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        public bool isActive = true;

        [Header("Default views")]
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
        [FolderPath]
        [GUIColor(nameof(ViewsBackgroundColor))]
#endif
        public List<string> uiViewsDefaultFolders = new();

        [Header("View Assets Sources")]
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
        [GUIColor(nameof(ViewsBackgroundColor))]
#endif
        public List<AssetReferenceGameObject> viewsAssetsSources = new();
        
        [Header("Skins folder")]
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
        [GUIColor(nameof(ViewsBackgroundColor))]
        //[BoxGroup(nameof(ViewsGroup))]
        [FolderPath]
#endif
        public List<string> uiViewsSkinFolders = new();

        [Header("Registered Views")]
#if ODIN_INSPECTOR
        [TabGroup(ViewTabKey)]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [GUIColor(nameof(ViewsBackgroundColor))]
        [PropertyOrder(-1)]
        [ListDrawerSettings(ListElementLabelName = "@ViewName")]
#endif
        public List<UiViewReference> uiViews = new();

        [Space]
#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
#endif
        public bool applyAddressablesGroup = false;

#if ODIN_INSPECTOR
#if UNITY_EDITOR
        [ValueDropdown(nameof(GetLabels), ExcludeExistingValuesInList = true)]
#endif
        [TabGroup(SettingsTabKey)]
        [ShowIf(nameof(applyAddressablesGroup))]
#endif
        public List<string> labels = new();

#if ODIN_INSPECTOR
        [TabGroup(SettingsTabKey)]
        [ShowIf(nameof(applyAddressablesGroup))]
#endif
        [Header("Name of target addressable group")]
        public string addressableGroupName;

        public bool IsActive => isActive;

        public IReadOnlyList<UiViewReference> Views => uiViews;

#if UNITY_EDITOR
        private IEnumerable<string> GetLabels()
        {
            return this.GetAllAddressableLabels();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button(ButtonSizes.Large)]
        [GUIColor(0.3f,0.8f,0.4f)]
#endif
        private void Rebuild()
        {
            MessageBroker.Default.Publish(new SettingsRebuildMessage() {ViewsSettings = this});
        }

#endif
    }
}