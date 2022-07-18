namespace UniGame.Lobby.Runtime.UI.ViewModels
{
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;

    public interface IBlackoutViewModel : IViewModel
    {
        ReactiveProperty<string> BottomText { get; }

        int SortingOrder { get; }
    }
}