namespace UniGame.UiSystem.Examples.ListViews.Views
{
    using Cysharp.Threading.Tasks;
    using Runtime;
    using TMPro;
    using UniModules.UniCore.Runtime.Utils;
    using UniRx;
    
    using UnityEngine.UI;
    using ViewModels;

    public class DemoItemView : UiView<DemoItemViewModel>
    {
        public TextMeshProUGUI level;
        public TextMeshProUGUI damage;
        public TextMeshProUGUI armor;
        public Image icon;

        public Button buyButton;
        public Button removeButton;
        
        protected override async UniTask OnInitialize(DemoItemViewModel model)
        {
            await base.OnInitialize(model);

            BindTo(model.Armor, x => armor.text   = x.ToStringFromCache());
            BindTo(model.Damage, x => damage.text = x.ToStringFromCache());
            BindTo(model.Level, x => level.text   = x.ToStringFromCache());
            BindTo(model.Icon, x => icon.sprite   = x);
            BindTo(buyButton.onClick.AsObservable(), x => model.Sell.Execute());
            BindTo(removeButton.onClick.AsObservable(),x => model.Remove.Execute());
        }
    }
}
