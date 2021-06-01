namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;
    using System.Collections.Generic;
    using Core.Runtime.Interfaces;
    using UnityEngine;

    public interface IReadOnlyViewLayout : 
        ILifeTimeContext
    {
        Transform          Layout    { get; }

        IObservable<IView> OnHidden  { get; }
        IObservable<IView> OnShown   { get; }
        IObservable<IView> OnHiding  { get; }
        IObservable<IView> OnShowing { get; }
        IObservable<IView> OnClosed  { get; }

        bool Contains(IView view);
 
        TView Get<TView>() where TView :class, IView;
        
        List<TView> GetAll<TView>() where TView : class, IView;

    }
}