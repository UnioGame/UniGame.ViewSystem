namespace UniModules.UniGame.UISystem.Runtime.WindowStackControllers.Abstract
{
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using Runtime.Abstract;
    using UnityEngine;

    public interface IViewLayoutFactory
    {
        IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView);
    }
}