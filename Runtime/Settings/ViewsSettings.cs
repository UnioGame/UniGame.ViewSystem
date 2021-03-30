using System.Linq;
using UniModules.UniGame.Core.Runtime.ScriptableObjects;

namespace UniGame.UiSystem.Runtime.Settings
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/ViewSystem/UiViewsSettings", fileName = "UiViewsSettings")]
    public class ViewsSettings : LifetimeScriptableObject, IViewsSettings
    {
        public string sourceName;

        [Header("Is Source will be filled on Update")]
        public bool isActive = true;

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

        [Header("Registered Views")] [Space] public List<UiViewReference> uiViews = new List<UiViewReference>();

        public bool IsActive => isActive;

        public IReadOnlyList<UiViewReference> Views => uiViews;
        
    }
}