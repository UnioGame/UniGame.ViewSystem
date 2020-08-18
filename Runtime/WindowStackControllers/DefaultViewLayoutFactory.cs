namespace UniModules.UniGame.UISystem.Runtime.WindowStackControllers
{
    using Abstract;
    using global::UniGame.UiSystem.Runtime;
    using global::UniGame.UiSystem.Runtime.Abstracts;
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/UiSystem/DefaultViewLayoutFactory", fileName = "DefaultViewLayoutFactory")]
    public class DefaultViewLayoutFactory : ViewLayoutFactoryAbstract
    {
        public override IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView)
        {
            return new DefaultViewLayout(canvasPoint, backgroundView);
        }
    }
}