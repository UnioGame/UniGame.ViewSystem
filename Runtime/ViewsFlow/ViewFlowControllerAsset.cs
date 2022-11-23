﻿namespace UniGame.UiSystem.Runtime.ViewsFlow
{
    using ViewSystem.Runtime;
    using UnityEngine;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.TypeInfoBox("Use This asset to specify cross layout flow")]
#endif
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Flow/Default",fileName = "DefaultFlow")]
    public class ViewFlowControllerAsset : ScriptableObject
    {
        public virtual IViewFlowController Create()
        {
            return new ViewFlowController();
        } 
    }
}
