﻿namespace UniModules.UniGame.UISystem.Runtime.WindowStackControllers
{
    using Abstract;
    using global::UniGame.UiSystem.Runtime;
    using global::UniGame.UiSystem.Runtime.Abstracts;
    using global::UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using UnityEngine;
    
    [CreateAssetMenu(menuName = "UniGame/UiSystem/StackViewLayoutFactory", fileName = "StackViewLayoutFactory")]
    public class StackViewLayoutFactory : ViewLayoutFactoryAbstract
    {
        public override IViewLayout Create(Transform canvasPoint, IBackgroundView backgroundView)
        {
            return new StackViewLayout(canvasPoint, backgroundView);
        }
    }
}