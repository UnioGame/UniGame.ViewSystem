namespace UniGame.Views.Backgrounds
{
    using UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using UnityEngine;

    public class DefaultBackgroundFactory : BackgroundFactory
    {
        [SerializeField]
        private DefaultBackgroundView _backgroundView;
        
        public override IBackgroundView Create(Transform parent)
        {
            var viewModel = new DefaultBackgroundViewModel();
            _backgroundView.Initialize(viewModel, _backgroundView.Layout);
            
            return _backgroundView;
        }
    }
}