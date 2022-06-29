namespace Taktika.UI.Common.ViewModels.Abstract
{
    using UnityEngine;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;

    public interface IMessagePanelViewModel : IViewModel
    {
        IReadOnlyReactiveProperty<string> MessageText { get; }

        Vector2 MessageOrigin { get; }
        MessagePanelSide Side { get; }

        Vector2 OriginOffset { get; }
    }
}