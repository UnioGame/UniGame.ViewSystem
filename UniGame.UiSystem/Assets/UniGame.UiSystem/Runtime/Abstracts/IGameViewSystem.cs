namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniCore.Runtime.Interfaces;
    using UniRx.Async;
    using UnityEngine;

    // Итерфейс непонятного предназначения 2 метода закрытия и больше ничего
    // здесь должны быть методы open/close которые отвечают не только за создание
    // но и за какую то логику расположения в иерархии сцен
    public interface IGameViewSystem : ILifeTimeContext, IViewElementFactory, IDisposable
    {
        bool CloseWindow<T>() where T :Component, IView;

        bool CloseScreen<T>() where T :Component, IView;
    }
}