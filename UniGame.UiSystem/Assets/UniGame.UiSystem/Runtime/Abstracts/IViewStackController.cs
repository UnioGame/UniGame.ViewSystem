namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UnityEngine;

    public interface IViewStackController : IDisposable
    {
        IObservable<IView> StackTopChanged { get; }

        Transform Layout { get; }

        bool Contains(IView view);
        
        void Add<TView>(TView view) where TView :Component, IView;

        TView Get<TView>() where TView : Component, IView;

        //bool Remove<T>(T view) where T : Component, IView;

        //void Close<T>() where T :Component, IView;

        void CloseAll();
    }
}