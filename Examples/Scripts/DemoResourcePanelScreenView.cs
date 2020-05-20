namespace UniGame.UiSystem.Examples.BaseUiManager.Scripts
{
    using Runtime;
    using TMPro;
    using UniGreenModules.UniCore.Runtime.Utils;

    public class DemoResourcePanelScreenView : WindowView<DemoResourceUiViewModel>
    {
        public TextMeshProUGUI goldValue;

        protected override void OnViewInitialize(DemoResourceUiViewModel model)
        {
            BindTo(model.Gold, x => goldValue.text = x.ToStringFromCache());
        }
    }
}
