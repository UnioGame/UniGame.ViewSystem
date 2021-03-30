using System;
using UniGame.UiSystem.Runtime.Settings;
using UniModules.UniGame.SerializableContext.Runtime.Addressables;

namespace UniModules.UniGame.ViewSystem.Runtime.Settings
{
    [Serializable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.DrawWithUnity]
#endif
    public class AssetReferenceViewSettings : AssetReferenceApiT<IViewsSettings>
    {
        public AssetReferenceViewSettings(string guid) : base(guid)
        {
        }
    }
}
