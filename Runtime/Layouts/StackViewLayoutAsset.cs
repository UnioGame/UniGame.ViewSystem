namespace UniGame.ViewSystem.Runtime.WindowStackControllers
{
    using Abstract;
    using global::UniGame.UiSystem.Runtime;
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using global::UniGame.ViewSystem.Runtime;
    using UnityEngine;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Layout/Stack View Layout", fileName = "Stack View Layout")]
    public class StackViewLayoutAsset : ViewLayoutType
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