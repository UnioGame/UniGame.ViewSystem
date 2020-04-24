namespace UniGame.UiSystem.Runtime.Settings
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/UiSystem/UiViewSource", fileName = "UiViewSource")]
    public class ViewsSource : ScriptableObject
    {
        public string sourceName;

        [HideInInspector] 
        [Header("Name of target addressable group")]
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

        [Space]
        public List<UiViewReference> uiViews = new List<UiViewReference>();
    }
}