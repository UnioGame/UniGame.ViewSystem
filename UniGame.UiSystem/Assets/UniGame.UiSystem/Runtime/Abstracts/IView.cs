﻿namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IView : ILifeTimeContext
    {
        IReadOnlyReactiveProperty<bool> IsActive { get; }
        
        bool IsDestroyed { get; }

        IObservable<IView> OnHidden { get; }

        IObservable<IView> OnShown { get; }

        IObservable<IView> OnClosed { get; }
        
        void Initialize(IViewModel vm,IViewProvider layouts);
        
        void Close();

        void Show();

        void Hide();

    }
}