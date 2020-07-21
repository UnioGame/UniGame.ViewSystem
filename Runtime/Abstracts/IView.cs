namespace UniGame.UiSystem.Runtime.Abstracts
{
    using Cysharp.Threading.Tasks;
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

        UniTask Initialize(IViewModel vm);
    }
}