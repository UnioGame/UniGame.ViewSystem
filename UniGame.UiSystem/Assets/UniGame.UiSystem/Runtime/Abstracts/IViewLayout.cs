namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UnityEngine;

    public interface IViewLayout : 
        IDisposable, 
        IReadOnlyViewLayout
    {
        Transform Layout { get; }

        void Push<TView>(TView view) where TView :Component, IView;

        bool Close<T>(T view) where T : Component, IView;
        
        
    }
}