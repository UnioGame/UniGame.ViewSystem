namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System.Collections.Generic;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UnityEngine;

    public interface IReadOnlyViewLayout : 
        ILifeTimeContext,
        IViewStatus
    {
        Transform Layout { get; }

        bool Contains(IView view);
 
        TView Get<TView>() where TView :class, IView;
        
        List<TView> GetAll<TView>() where TView : class, IView;

    }
}