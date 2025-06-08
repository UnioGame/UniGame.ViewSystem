namespace UniGame.Lobby.Runtime.UI.ViewModels
{
    using R3;
    using ViewSystem.Runtime;
     

    public interface IBlackoutViewModel : IViewModel
    {
        ReactiveProperty<string> BottomText { get; }

        int SortingOrder { get; }
    }
}