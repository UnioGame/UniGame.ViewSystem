namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UnityEngine;

    public interface IViewLayout : 
        IDisposable, 
        IReadOnlyViewLayout
    {
        Transform Layout { get; }

        void Add<TView>(TView view) where TView :Component, IView;

        bool Remove<T>(T view) where T : Component, IView;
        
        
    }
}