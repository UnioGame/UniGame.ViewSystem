namespace Taktika.Lobby.Runtime.UI.Views
{
    using System;
    using UniGame.Localization.Runtime.UniModules.UniGame.Localization.Runtime;
    using UniGame.UiSystem.Runtime;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine.Localization;

    public class SmartArrowHintViewModel : ViewModelBase, IArrowHintViewModel
    {
        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        private string   _cachedFormat;
        private string[] _cachedSmartValues;
        
        public IReadOnlyReactiveProperty<string> Text => _text;

        public SmartArrowHintViewModel(LocalizedString locReference, IObservable<string[]> smartValues)
        {
            _text.Value = string.Empty;
            
            locReference.BindChangeHandler(x =>
            {
                _cachedFormat = x;
                UpdateText(x, _cachedSmartValues);
            }).AddTo(LifeTime);

            smartValues.Subscribe(x =>
            {
                _cachedSmartValues = x;
                UpdateText(_cachedFormat, x);
            }).AddTo(LifeTime);
        }

        private void UpdateText(string format, string[] smartValues)
        {
            if(smartValues == null || string.IsNullOrEmpty(format))
                return;
            
            _text.Value = string.Format(format, smartValues);
        }
    }
}