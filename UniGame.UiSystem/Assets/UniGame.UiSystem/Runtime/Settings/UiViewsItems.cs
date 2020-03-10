namespace UniGreenModules.UniGame.UiSystem.Runtime.Settings
{
    using System;
    using System.Collections.Generic;
    using AddressableTools.Runtime.Attributes;
    using Sirenix.OdinInspector;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class UiViewsItems
    {
        public string viewTag;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [ShowAssetReference]
        public List<AssetReferenceGameObject> views = new List<AssetReferenceGameObject>();
    }
}