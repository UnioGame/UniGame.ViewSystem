namespace Taktika.UI.Common.ViewModels
{
    using System;
    using UniGame.UiSystem.Runtime;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;

    public class ButtonViewModel : ViewModelBase, IButtonViewModel
    {
        private readonly IObservable<Sprite> _icon;
        private readonly IObservable<string> _label;
        private readonly ReactiveCommand     _command;

        public ButtonViewModel(Action buttonAction,IObservable<bool> canExecute = null, IObservable<Sprite> icon = null, IObservable<string> label = null) : this(buttonAction,icon,label,canExecute)
        {
            
        }
        
        public ButtonViewModel(Action buttonAction, IObservable<Sprite> icon = null, IObservable<string> label = null,IObservable<bool> canExecute = null)
        {
            _icon         = icon;
            _label        = label;

            _command = new ReactiveCommand().AddTo(LifeTime);
            _command.Subscribe(x => buttonAction?.Invoke())
                .AddTo(LifeTime);
        }

        public IReadOnlyReactiveProperty<bool> Interactable => Activate.CanExecute;

        public IReactiveCommand<Unit> Activate => _command;

        public IObservable<Sprite> Icon => _icon ??  Observable.Empty<Sprite>();

        public IObservable<string> Label => _label ?? Observable.Empty<string>();

    }
    
    
    
}