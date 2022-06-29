namespace Taktika.UI.Common.ViewModels
{
    using System;
    using UniRx;
    using UnityEngine;

    public readonly struct ButtonViewModelData
    {
        public readonly IObserver<Unit>     ButtonAction;
        public readonly IObservable<Sprite> Icon;
        public readonly IObservable<string> Label;
        public readonly IObservable<bool>   CanExecute;
        
        public ButtonViewModelData(
            IObserver<Unit> buttonAction,
            IObservable<Sprite> icon = null, 
            IObservable<string> label = null,
            IObservable<bool> canExecute = null)
        {
            ButtonAction = buttonAction;
            Icon         = icon;
            Label        = label;
            CanExecute   = canExecute;
        }
    }
}