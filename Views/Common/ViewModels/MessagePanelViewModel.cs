namespace Taktika.UI.Common.ViewModels
{
    using Abstract;
    using UniGame.Localization.Runtime.UniModules.UniGame.Localization.Runtime;
    using UniGame.UiSystem.Runtime;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Localization;

    public class MessagePanelViewModel : ViewModelBase, IMessagePanelViewModel
    {
        private readonly ReactiveProperty<string> _messageTextProperty;
        public IReadOnlyReactiveProperty<string> MessageText => _messageTextProperty;

        public Vector2 OriginOffset { get; }

        public Vector2 MessageOrigin { get; }
       
        public MessagePanelSide Side { get; }

        public MessagePanelViewModel(LocalizedString messageText, Vector2 messageOrigin, Vector2 originOffset, MessagePanelSide side)
        {
            _messageTextProperty = new ReactiveProperty<string>(string.Empty);
            _messageTextProperty.AddTo(LifeTime);

            MessageOrigin = messageOrigin;
            OriginOffset = originOffset;

            Side = side;
            
            messageText.BindChangeHandler(newText => _messageTextProperty.Value = newText);
        }
    }
}