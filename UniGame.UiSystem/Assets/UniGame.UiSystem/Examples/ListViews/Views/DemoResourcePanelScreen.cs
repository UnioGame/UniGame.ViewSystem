namespace UniGame.UiSystem.Examples.ListViews.Views
{
    using BaseUiManager.Scripts;
    using Runtime;
    using TMPro;
    using UniGreenModules.UniCore.Runtime.Utils;

    public class DemoResourcePanelScreen : WindowView<DemoResourceUiViewModel>
    {
        public TextMeshProUGUI goldValue;

        protected override void OnViewInitialize(DemoResourceUiViewModel model)
        {
            BindTo(model.Gold, x => goldValue.text = x.ToStringFromCache());
        }
    }
}
