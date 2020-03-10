namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniCore.Runtime.Interfaces;
    using UniRx.Async;
    using UnityEngine;

    public interface IGameViewSystem : ILifeTimeContext, IViewElementFactory, IDisposable
    {
        bool CloseWindow<T>() where T :Component, IView;

        bool CloseScreen<T>() where T :Component, IView;
    }
}