namespace UniModules.UniGame.UISystem.Runtime.WindowStackControllers
{
    using Abstract;
    using global::UniGame.UiSystem.Runtime;
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using global::UniGame.ViewSystem.Runtime;
    using Sirenix.OdinInspector;
    using UnityEngine;
    
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/StackViewLayoutFactory", fileName = "StackViewLayoutFactory")]
    public class StackViewLayoutFactory : ViewLayoutType
    {
#if ODIN_INSPECTOR
        [InlineProperty]
        [HideLabel]
#endif
        public StackViewLayout layout;
        
        public override IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView)
        {
            layout.Initialize(canvasPoint,backgroundView);
            return layout;
        }
    }
}