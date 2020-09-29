namespace UniModules.UniGame.UISystem.Runtime.WindowStackControllers.Abstract
{
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using Runtime.Abstract;
    using UnityEngine;

    public abstract class ViewLayoutFactoryAbstract : ScriptableObject, IViewLayoutFactory
    {
        public abstract IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView);
    }
}