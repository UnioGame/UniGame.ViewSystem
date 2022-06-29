namespace Taktika.UI.Common.ViewModels
{
    using Abstract;
    using UniGame.UiSystem.Runtime;
    using UniRx;
    using UnityEngine.Localization;
    using UniGame.Localization.Runtime.UniModules.UniGame.Localization.Runtime;

    public class MessageBubbleViewModel : ViewModelBase, IMessageBubbleViewModel
    {
        private ReactiveProperty<string> _text = new ReactiveProperty<string>();
        public IReadOnlyReactiveProperty<string> Text => _text;

        public MessageBubbleViewModel(LocalizedString message) 
        {
            _text.Value = string.Empty;
            message.BindChangeHandler(val => _text.Value = val);
        }
    }
}
