namespace UniGame.ViewSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using global::UniGame.Core.Runtime;
    using UnityEngine;

    public interface IReadOnlyViewLayout : 
        ILifeTimeContext
    {
        Transform          Layout    { get; }

        IObservable<IView> OnHidden  { get; }
        IObservable<IView> OnShown   { get; }
        IObservable<IView> OnBeginHide  { get; }
        IObservable<IView> OnBeginShow { get; }
        IObservable<IView> OnClosed  { get; }

        IObservable<Type> OnIntent { get; }

        bool Contains(IView view);
 
        TView Get<TView>() where TView :class, IView;
        
        List<TView> GetAll<TView>() where TView : class, IView;

    }
}