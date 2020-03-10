namespace UniGreenModules.UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IViewModel: IDisposable, ILifeTimeContext
    {
        IReadOnlyReactiveProperty<bool> IsActive { get; }
        
    }
}