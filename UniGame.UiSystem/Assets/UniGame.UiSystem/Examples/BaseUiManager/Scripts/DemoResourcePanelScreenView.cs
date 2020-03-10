namespace UniGreenModules.UniGame.UiSystem.Examples.BaseUiManager.Scripts
{
    using Runtime;
    using Runtime.Extensions;
    using TMPro;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Utils;

    public class DemoResourcePanelScreenView : WindowView<DemoResourceUiViewModel>
    {
        public TextMeshProUGUI goldValue;

        protected override void OnWindowInitialize(DemoResourceUiViewModel model, ILifeTime lifeTime)
        {
            model.Bind(model.Gold, x => goldValue.text = x.ToStringFromCache());
        }
    }
}
