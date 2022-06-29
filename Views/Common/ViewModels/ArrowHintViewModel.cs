namespace Taktika.Lobby.Runtime.UI.Views
{
    using System;
    using UniGame.Localization.Runtime.UniModules.UniGame.Localization.Runtime;
    using UniGame.UiSystem.Runtime;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine.Localization;

    public class ArrowHintViewModel : ViewModelBase, IArrowHintViewModel
    {
        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        public IReadOnlyReactiveProperty<string> Text => _text;

        public ArrowHintViewModel()
        {
            _text.Value = string.Empty;
        }

        public ArrowHintViewModel(LocalizedString locReference)
        {
            _text.Value = String.Empty;
            locReference.BindChangeHandler(v=>_text.Value = v).AddTo(LifeTime);
        }
    }
}