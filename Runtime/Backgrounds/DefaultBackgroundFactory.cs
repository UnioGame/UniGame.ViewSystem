using Cysharp.Threading.Tasks;
using Taktika.UI.Backgrounds;

namespace UniGame.ViewSystem.Backgrounds
{
    using UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using UnityEngine;

    public class DefaultBackgroundFactory : BackgroundFactory
    {
        [SerializeField]
        public DefaultFadeBackgroundView backgroundView;

        public bool createInstance = true;

        private DefaultFadeBackgroundView _view;
        
        public override IBackgroundView Create(Transform parent)
        {
            var viewModel = new DefaultBackgroundViewModel();

            _view = _view ? _view : createInstance
                ? Instantiate(backgroundView.gameObject, parent).GetComponent<DefaultFadeBackgroundView>()
                : backgroundView;

            _view.Initialize(viewModel,_view.Layout).Forget();
            
            return _view;
        }
    }
}