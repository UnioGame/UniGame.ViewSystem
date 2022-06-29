namespace Taktika.UI.Common.ViewModels.Abstract
{
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;

    public interface IMessageBubbleViewModel : IViewModel
    {
        IReadOnlyReactiveProperty<string> Text { get; }
    }
}
