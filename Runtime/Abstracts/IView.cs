namespace UniGame.UiSystem.Runtime.Abstracts
{
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    public interface IView : 
        ILifeTimeContext, 
        IViewStatus, 
        IViewCommands
    {
        IReadOnlyReactiveProperty<bool> IsActive { get; }

        RectTransform RectTransform { get; }

        bool IsTerminated { get; }

        void Initialize(IViewModel vm);
    }
}