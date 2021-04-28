using System;
using System.Collections.Generic;
using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    [Serializable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ListDrawerSettings]
#endif
    public class UiReferenceList : List<UiViewReference>
    {
        
    }
}