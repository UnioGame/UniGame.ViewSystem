namespace UniGreenModules.UniGame.UiSystem.Runtime.Settings
{
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/UiSystem/UiViewSource", fileName = "UiViewSource")]
    public class UiViewsSource : ScriptableObject
    {
        public string sourceName;

        [HideInInspector] 
        [Header("Name of target addressable group")]
        public string addressableGroupName;

        [Space]
        [Header("Default views")]
#if ODIN_INSPECTOR
        [FolderPath]
#endif
        public List<string> uiViewsDefaultFolders = new List<string>();

        [Header("Skins folder")]
#if ODIN_INSPECTOR
        [FolderPath]
#endif
        public List<string> uiViewsSkinFolders = new List<string>();

        [Space]
        public List<UiViewDescription> uiViews = new List<UiViewDescription>();
    }
}