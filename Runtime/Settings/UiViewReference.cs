namespace UniGame.UiSystem.Runtime.Settings
{
    using System;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UniModules.UniGame.Core.Runtime.SerializableType;
    using UniModules.UniGame.Core.Runtime.SerializableType.Attributes;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class UiViewReference
    {
        public string AssetGUID = string.Empty;

        [Space(2)] public AssetReferenceGameObject View;
        
        [Space(2)]
        public string ViewName;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.GUIColor(g: 1.0f, r: 1.0f, b: 0.5f)]
#endif
        [Space(2)]
        public string Tag = string.Empty;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [Space(2)]
        [STypeFilter(typeof(IView))]
        public SType Type;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelText("@GetModelTypeName()")]
#endif
        [Space(2)]
        public SType ModelType;

        //type of view model
        [Space(2)]
        public SType ViewModelType;
        
        private string GetModelTypeName()
        {
            return $"ModelType : {ModelType?.Type?.GetFormattedName()}";
        }

        
        
    }
}