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
            this.
                Bind(model.Armor, x => armor.text = x.ToStringFromCache()).
                Bind(model.Damage, x => damage.text = x.ToStringFromCache()).
                Bind(model.Level, x => level.text = x.ToStringFromCache()).
                Bind(model.Icon, x => icon.sprite = x).
                Bind(buyButton.onClick.AsObservable(), model.Sell).
                Bind(removeButton.onClick.AsObservable(), model.Remove);
        }
    }
}
