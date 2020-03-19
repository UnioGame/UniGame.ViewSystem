namespace UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IView : ILifeTimeContext, IViewStatus
    {
        IReadOnlyReactiveProperty<bool> IsActive { get; }
        
        bool IsDestroyed { get; }

        void Initialize(IViewModel vm,IViewProvider layouts);
        
        void Close();

        void Show();

        void Hide();

    }
}