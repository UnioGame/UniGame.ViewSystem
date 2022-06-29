namespace Taktika.UI.Views
{
    using Abstract;
    using Cysharp.Threading.Tasks;
    using UniModules.Rx.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.UiSystem.Runtime.Extensions;
    using UniModules.UniUiSystem.Runtime.Utils;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.UI;
    using ViewModels.Abstract;

    public class NoInternetView : UiAnimatorView<INoInternetViewModel>, INoInternetView
    {
        public Image  iconBackgroundImage;
        public Image  titleImage;
        public Image  backgroundImage;
        public Image  iconImage;
        public Button closeButton;
        public Button retryButton;

        protected sealed override UniTask OnViewInitialize(INoInternetViewModel model)
        {
            this.Bind(closeButton, OnClose)
                .Bind(retryButton, OnRetry);
            
            return UniTask.CompletedTask;
        }

        private void OnClose()
        {
            Close();
            Model.Cancel();
        }

        private void OnRetry()
        {
            Close();
            Model.TryAgain();
        }
    }
}