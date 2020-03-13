namespace UniGame.UiSystem.Examples.BaseUiManager.Scripts
{
    using Runtime;
    using TMPro;
    using UniGreenModules.UniCore.Runtime.Utils;
    using UniGreenModules.UniGame.UiSystem.Runtime;

    public class DemoResourcePanelScreenView : WindowView<DemoResourceUiViewModel>
    {
        public TextMeshProUGUI goldValue;

        protected override void OnWindowInitialize(DemoResourceUiViewModel model)
        {
            BindTo(model.Gold, x => goldValue.text = x.ToStringFromCache());
        }
    }
}
