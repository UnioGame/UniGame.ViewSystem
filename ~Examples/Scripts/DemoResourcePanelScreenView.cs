namespace UniGame.UiSystem.Examples.BaseUiManager.Scripts
{
    using Cysharp.Threading.Tasks;
    using Runtime;
    using TMPro;
    using UniModules.UniCore.Runtime.Utils;
    

    public class DemoResourcePanelScreenView : WindowView<DemoResourceUiViewModel>
    {
        public TextMeshProUGUI goldValue;

        protected override async UniTask OnViewInitialize(DemoResourceUiViewModel model)
        {
            await base.OnViewInitialize(model);
            
            BindTo(model.Gold, x => goldValue.text = x.ToStringFromCache());
        }
    }
}
