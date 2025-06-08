namespace UniGame.ViewSystem.Runtime
{
    using System;
    using System.Collections.Generic;
    using global::UniGame.Core.Runtime;
    using R3;
    using UnityEngine;

    public interface IReadOnlyViewLayout : 
        ILifeTimeContext
    {
        Transform          Layout    { get; }

        Observable<IView> OnHidden  { get; }
        Observable<IView> OnShown   { get; }
        Observable<IView> OnBeginHide  { get; }
        Observable<IView> OnBeginShow { get; }
        Observable<IView> OnClosed  { get; }

        Observable<Type> OnIntent { get; }

        bool Contains(IView view);
 
        TView Get<TView>() where TView :class, IView;
        
        List<TView> GetAll<TView>() where TView : class, IView;

    }
}