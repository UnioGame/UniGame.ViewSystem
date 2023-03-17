namespace UniModules.UniGame.UISystem.Runtime.WindowStackControllers.Abstract
{
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using global::UniGame.ViewSystem.Runtime;
    using UnityEngine;

    public interface IViewLayoutFactory
    {
        IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView);
    }
}