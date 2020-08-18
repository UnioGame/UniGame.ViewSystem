namespace UniModules.UniGame.UISystem.Runtime.WindowStackControllers.Abstract
{
    using global::UniGame.UiSystem.Runtime.Abstracts;
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using UnityEngine;

    public interface IViewLayoutFactory
    {
        IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView);
    }
}