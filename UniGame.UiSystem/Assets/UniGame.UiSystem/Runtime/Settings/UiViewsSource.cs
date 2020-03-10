namespace UniGreenModules.UniGame.UiSystem.Runtime.Settings
{
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Taktika/Ui/UiViewSource", fileName = "UiViewSource")]
    public class UiViewsSource : ScriptableObject
    {
        public string sourceName;

        [HideInInspector]
        [Header("Name of target addressable group")]
        public string addressableGroupName;
        
        [Space]
        [Header("Default views")]
        [FolderPath]
        public List<string> uiViewsDefaultFolders = new List<string>();

        [Header("Skins folder")]
        [FolderPath]
        public List<string> uiViewsSkinFolders = new List<string>();

        [Space]
        public List<UiViewDescription> uiViews = new List<UiViewDescription>();
        
    }
}