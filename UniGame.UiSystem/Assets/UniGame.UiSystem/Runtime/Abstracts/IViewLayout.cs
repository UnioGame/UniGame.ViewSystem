namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UnityEngine;

    public interface IViewLayout : 
        IDisposable, 
        IViewStatus,
        IReadOnlyViewLayout,
        ILifeTimeContext
    {
        Transform Layout { get; }

        void Push<TView>(TView view) where TView :Component, IView;

        bool Close<T>(T view) where T : Component, IView;
        
    }
}