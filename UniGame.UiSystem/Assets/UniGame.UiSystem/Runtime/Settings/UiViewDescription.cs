namespace UniGreenModules.UniGame.UiSystem.Runtime.Settings
{
    using System;
    using Abstracts;
    using AddressableTools.Runtime.Attributes;
    using Core.Runtime.SerializableType;
    using Core.Runtime.SerializableType.Attributes;
    using Sirenix.OdinInspector;
    using Taktika.GameRuntime.Types;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class UiViewDescription
    {
            
#if ODIN_INSPECTOR
        [GUIColor(g:1.0f, r: 1.0f, b:0.5f)]
#endif
        [Space(2)]
        public string Tag = string.Empty;
      
#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        [Space(2)]
        [STypeFilter(typeof(IView), nameof(SType.fullTypeName))]
        public SType Type;

#if ODIN_INSPECTOR
        [DrawWithUnity]
#endif
        [Space(2)]
        [ShowAssetReference]
        public AssetReferenceGameObject View;

    }
}