using System;

namespace UniGame.UiSystem.Runtime.Settings
{
    [Serializable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    public class NestedViewSourceSettings
    {
        public bool awaitLoading = false;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        public AssetReferenceViewSource viewSourceReference;
    }
}