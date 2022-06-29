namespace Taktika.Lobby.Runtime.UI.ViewModels
{
    using UniGame.Localization.Runtime.UniModules.UniGame.Localization.Runtime;
    using UniGame.UiSystem.Runtime;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine.Localization;

    public class BlackoutViewModel : ViewModelBase, IBlackoutViewModel
    {
        public ReactiveProperty<string> BottomText { get; }

        public int SortingOrder { get; }

        public BlackoutViewModel(int sortingOrder, string bottomText = "")
        {
            SortingOrder = sortingOrder;
            
            BottomText = new ReactiveProperty<string>(bottomText);
            BottomText.AddTo(LifeTime);
        }

        public BlackoutViewModel(int sortingOrder, LocalizedString locReference)
        {
            SortingOrder = sortingOrder;

            BottomText = new ReactiveProperty<string>(string.Empty);

            locReference.BindChangeHandler(v => BottomText.Value = v).AddTo(LifeTime);
        }
    }
}