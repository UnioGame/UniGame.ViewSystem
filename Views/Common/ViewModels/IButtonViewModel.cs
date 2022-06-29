namespace Taktika.UI.Common.ViewModels
{
    using System;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    using UnityEngine;

    public interface IButtonViewModel : IViewModel
    {
        IReactiveCommand<Unit> Activate     { get; }
        IReadOnlyReactiveProperty<bool>      Interactable { get; }
        IObservable<Sprite>    Icon         { get; }
        IObservable<string>    Label        { get; }
    }
}