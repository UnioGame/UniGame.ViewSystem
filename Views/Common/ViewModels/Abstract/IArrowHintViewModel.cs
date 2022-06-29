namespace Taktika.Lobby.Runtime.UI.Views
{
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;

    public interface IArrowHintViewModel : IViewModel
    {
        IReadOnlyReactiveProperty<string> Text { get; }
    }
}