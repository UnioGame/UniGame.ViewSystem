namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using Abstracts;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Attributes;
    using UniGreenModules.UniGame.Core.Runtime.SerializableType;
    using UniGreenModules.UniGame.Core.Runtime.SerializableType.Attributes;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class UiViewReference
    {
            
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.GUIColor(g:1.0f, r: 1.0f, b:0.5f)]
#endif
        [Space(2)]
        public string Tag = string.Empty;
      
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [Space(2)]
        [STypeFilter(typeof(IView), nameof(SType.fullTypeName))]
        public SType Type;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [Space(2)]
        [ShowAssetReference]
        public AssetReferenceGameObject View;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [Space(2)]
        public string ViewName;

    }
}