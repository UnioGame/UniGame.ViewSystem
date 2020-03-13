namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UnityEngine;

    public interface IViewLayoutController : 
        IDisposable, 
        IReadOnlyViewLayoutController
    {
        Transform Layout { get; }

        void Add<TView>(TView view) where TView :Component, IView;

        bool Remove<T>(T view) where T : Component, IView;
    }
}