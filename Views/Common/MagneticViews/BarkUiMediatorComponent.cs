namespace Taktika.Lobby.Runtime.UI.Views
{
    using PixelCrushers.DialogueSystem;

    public class BarkUiMediatorComponent : AbstractBarkUI
    {
        private IBarkUI _barkUiView;

        public void SetTargetObject(IBarkUI barkUiView)
        {
            this._barkUiView = barkUiView;
        }
        public override void Hide()
        {
            _barkUiView.Hide();
        }

        public override bool isPlaying => _barkUiView.isPlaying;
        public override void Bark(Subtitle subtitle)
        {
            _barkUiView.Bark(subtitle);
        }
    }
}