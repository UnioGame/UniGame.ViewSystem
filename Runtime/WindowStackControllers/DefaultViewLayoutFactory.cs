﻿namespace UniModules.UniGame.UISystem.Runtime.WindowStackControllers
{
    using Abstract;
    using global::UniGame.UiSystem.Runtime;
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using global::UniGame.ViewSystem.Runtime;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/ViewSystem/DefaultViewLayoutFactory", fileName = "DefaultViewLayoutFactory")]
    public class DefaultViewLayoutFactory : ViewLayoutType
    {
        public override IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView)
        {
            return new DefaultViewLayout(canvasPoint, backgroundView);
        }
    }
}