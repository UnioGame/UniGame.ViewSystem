namespace UniModules.UniGame.UISystem.Runtime.WindowStackControllers
{
    using Abstract;
    using global::UniGame.UiSystem.Runtime;
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using global::UniGame.ViewSystem.Runtime;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Layout/Single View Layout", fileName = "Single View Layout")]
    public class SingleViewLayoutAsset : ViewLayoutType
    {
#if ODIN_INSPECTOR
        [InlineProperty]
        [HideLabel]
#endif
        public SingleViewLayout layout;
        
        public override IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView)
        {
            var instance = Instantiate(this);
            var newLayout = instance.layout;
            newLayout.Initialize(canvasPoint,backgroundView);
            return newLayout;
        }
    }
}