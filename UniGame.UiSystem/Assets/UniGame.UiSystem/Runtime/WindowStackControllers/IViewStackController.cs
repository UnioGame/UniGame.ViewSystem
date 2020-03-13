namespace UniGame.UiSystem.Runtime
{
    using System;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UnityEngine;

    public interface IViewStackController<TV> where TV : IView
    {
        IObservable<IView> StackTopChanged { get; }
        void            Push(TV view);
        T               Get<T>() where T : Component, IView;
        void            CloseAll();
    }
}