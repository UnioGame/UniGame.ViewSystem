namespace Taktika.UI.Common.Views
{
    using System;
    using Cysharp.Threading.Tasks;
    using ViewModels;
    using Taktika.UI.Views;
    using UniModules.Rx.Extensions;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameUpdateView : UiAnimatorView<GameUpdateViewModel>
    {
        [SerializeField] private Button _downloadButton;

        public IObservable<Unit> UpdateButtonClicked => _downloadButton.OnClickAsObservable();
        
        protected override UniTask OnViewInitialize(GameUpdateViewModel model)
        {
            this.Bind(_downloadButton.OnClickAsObservable(), _ => Model?.OpenStore());
            return UniTask.CompletedTask;
        }
    }
}