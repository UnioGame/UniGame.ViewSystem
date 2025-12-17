namespace UniGame.UiSystem.Runtime.ViewsFlow
{
    using System;
    using System.Collections.Generic;
    using UniGame.Runtime.Utils;
    using UniModules.UniGame.UiSystem.Runtime;
    using ViewSystem.Runtime;
    using UnityEngine;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.TypeInfoBox("Use This asset to specify cross layout flow")]
#endif
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Flow/Simple",fileName = "Simple Flow")]
    public class SimpleFlowControllerAsset : BaseFlowControllerAsset
    {
        public SimpleFlowSettings settings = new();
        
        public override IViewFlowController Create()
        {
            return new SimpleFlowController(settings);
        } 
    }
    
    [Serializable]
    public class SimpleFlowSettings
    {
        public List<SimpleFlowLayoutSettings> Layouts = new()
        {
            new SimpleFlowLayoutSettings()
            {
                Layout = ViewType.Screen.ToStringFromCache(),
                CloseOnSceneLoad =  false,
            },
            new SimpleFlowLayoutSettings()
            {
                Layout = ViewType.Window.ToStringFromCache(),
                CloseOnSceneLoad =  false,
            },
            new SimpleFlowLayoutSettings()
            {
                Layout = ViewType.Overlay.ToStringFromCache(),
                CloseOnSceneLoad =  false,
            },
        };
    }
    
    [Serializable]
    public class SimpleFlowLayoutSettings
    {
        public string Layout = string.Empty;
        public bool CloseOnSceneLoad = false;
    }
}
