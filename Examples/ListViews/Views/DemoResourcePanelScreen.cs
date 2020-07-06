namespace UniGame.UiSystem.Examples.ListViews.Views
{
    using BaseUiManager.Scripts;
    using Runtime;
    using TMPro;
    using UniGreenModules.UniCore.Runtime.Utils;
    using UniRx.Async;

    public class DemoResourcePanelScreen : WindowView<DemoResourceUiViewModel>
    {
        public TextMeshProUGUI goldValue;

        protected override async UniTask OnViewInitialize(DemoResourceUiViewModel model)
        {
            await base.OnViewInitialize(model);
            
            BindTo(model.Gold, x => goldValue.text = x.ToStringFromCache());
        }
    }
}
