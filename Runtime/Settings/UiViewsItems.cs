namespace UniModules.UniGame.UiSystem.Runtime.Settings
{
    using System;
    using System.Collections.Generic;
    using AddressableTools.Runtime.Attributes;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class UiViewsItems
    {
        public string viewTag;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [DrawAssetReference]
        public List<AssetReferenceGameObject> views = new List<AssetReferenceGameObject>();
    }
}