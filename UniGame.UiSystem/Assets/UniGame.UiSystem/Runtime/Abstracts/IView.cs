namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    public interface IView : ILifeTimeContext, IViewStatus
    {
        IReadOnlyReactiveProperty<bool> IsActive { get; }

        RectTransform RectTransform { get; }

        bool IsTerminated { get; }

        void Initialize(IViewModel vm,IViewProvider layouts);

        void Destroy();
        
        void Close();

        void Show();

        void Hide();

    }
}