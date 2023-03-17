namespace UniGame.Lobby.Runtime.UI.ViewModels
{
    using ViewSystem.Runtime;
    using UniRx;

    public interface IBlackoutViewModel : IViewModel
    {
        ReactiveProperty<string> BottomText { get; }

        int SortingOrder { get; }
    }
}