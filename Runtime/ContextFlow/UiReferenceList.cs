using System;
using System.Collections.Generic;
using UniGame.UiSystem.Runtime.Settings;

namespace UniGame.ViewSystem.Runtime
{
    [Serializable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ListDrawerSettings]
#endif
    public class UiReferenceList
    {
        public List<UiViewReference> references = new List<UiViewReference>(16);
    }
}