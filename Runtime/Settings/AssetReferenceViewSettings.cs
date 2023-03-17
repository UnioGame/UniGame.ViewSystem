using System;
using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem.Runtime.Settings
{
    using UnityEngine.AddressableAssets;

    [Serializable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.DrawWithUnity]
#endif
    public class AssetReferenceViewSettings : AssetReferenceT<ViewsSettings>
    {
        public AssetReferenceViewSettings(string guid) : base(guid)
        {
        }
    }
}
