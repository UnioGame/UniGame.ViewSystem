namespace UniModules.UniGame.UISystem.Runtime.WindowStackControllers
{
    using Abstract;
    using global::UniGame.UiSystem.Runtime;
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using global::UniGame.ViewSystem.Runtime;
    using UnityEngine;
    
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/StackViewLayoutFactory", fileName = "StackViewLayoutFactory")]
    public class StackViewLayoutFactory : ViewLayoutType
    {
        public override IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView)
        {
            return new StackViewLayout(canvasPoint, backgroundView);
        }
    }
}