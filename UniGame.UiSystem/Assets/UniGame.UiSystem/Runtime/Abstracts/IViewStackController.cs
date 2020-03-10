namespace UniGreenModules.UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UnityEngine;

    public interface IViewStackController : IDisposable
    {
        bool Contains(IView view);
        
        void Add<TView>(TView view) where TView :Component, IView;

        void Hide<T>() where T :Component, IView;

        void HideAll();

        void HideAll<T>() where T : Component, IView;
        
        void Close<T>() where T :Component, IView;
        
        void CloseAll();

        bool Close<T>(T view) where T : Component, IView;

    }
}