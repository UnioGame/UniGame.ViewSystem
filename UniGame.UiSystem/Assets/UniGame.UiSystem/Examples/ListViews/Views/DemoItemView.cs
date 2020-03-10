using UniGreenModules.UniGame.UiSystem.Examples.ListViews.ViewModels;
using UniGreenModules.UniGame.UiSystem.Runtime;

namespace UniGreenModules.UniGame.UiSystem.Examples.ListViews.Views
{
    using Runtime.Extensions;
    using TMPro;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Utils;
    using UniRx;
    using UnityEngine.UI;

    public class DemoItemView : UiView<DemoItemViewModel>
    {
        public TextMeshProUGUI level;
        public TextMeshProUGUI damage;
        public TextMeshProUGUI armor;
        public Image icon;

        public Button buyButton;
        public Button removeButton;
        
        protected override void OnInitialize(DemoItemViewModel model, ILifeTime lifeTime)
        {
            BindTo(model.Armor, x => armor.text = x.ToStringFromCache()).
            BindTo(model.Damage, x => damage.text = x.ToStringFromCache()).
            BindTo(model.Level, x => level.text = x.ToStringFromCache()).
            BindTo(model.Icon, x => icon.sprite = x).
            BindTo(buyButton.onClick.AsObservable(),x => model.Sell.Execute()).
            BindTo(removeButton.onClick.AsObservable(),x => model.Remove.Execute());
        }
    }
}
