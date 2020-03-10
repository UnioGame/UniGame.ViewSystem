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
        [GUIColor(g:1.0f, r: 1.0f, b:0.5f)]
        [Space(2)]
        public string Tag = string.Empty;
        
        [Space(2)]
        [DrawWithUnity]
        [STypeFilter(typeof(IView), nameof(SType.fullTypeName))]
        public SType Type;

        [Space(2)]
        [DrawWithUnity]
        [ShowAssetReference]
        public AssetReferenceGameObject View;

    }
}