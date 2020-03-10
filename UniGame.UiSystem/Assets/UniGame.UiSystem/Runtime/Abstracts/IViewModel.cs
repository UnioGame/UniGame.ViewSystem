namespace UniGreenModules.UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IViewModel: IDisposable, ILifeTimeContext
    {
        // по смыслу проперти - активна ли view,
        // но VM это объект преобразующий понятия модели в понятия понятные view
        // а видимость не видимость определенной view не возможно заключить из модели 
        // поэтому этот флаг предлагаю отсюда убрать
        IReadOnlyReactiveProperty<bool> IsActive { get; }
        
    }
}