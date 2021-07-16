namespace Taktika.UI.Backgrounds
{
    using UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using UnityEngine;

    public class DefaultBackgroundFactory : BackgroundFactory
    {
        [SerializeField]
        private DefaultFadeBackgroundView _backgroundView;
        
        public override IBackgroundView Create()
        {
            var viewModel = new DefaultBackgroundViewModel();
            _backgroundView.Initialize(viewModel, _backgroundView.Layout);
            
            return _backgroundView;
        }
    }
}