namespace UniGame.ViewSystem.Runtime.WindowStackControllers.Abstract
{
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using global::UniGame.ViewSystem.Runtime;
    using UnityEngine;

    public abstract class ViewLayoutType : ScriptableObject, IViewLayoutFactory
    {
        public abstract IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView);
    }
}