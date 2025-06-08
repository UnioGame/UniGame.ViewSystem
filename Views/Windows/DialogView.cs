namespace UniGame.ViewSystem.Views.Windows
{
    using System;
    using Cysharp.Threading.Tasks;
    using R3;
    using UniGame.Runtime.Rx.Runtime.Extensions;
    using UniGame.UiSystem.Runtime;
     
    using UnityEngine;
    using UnityEngine.UI;
    
    public class DialogView<TModel> : View<TModel>
        where TModel : DialogViewModel
    {
        private readonly ReactiveCommand _onYesButtonClick = new ReactiveCommand();
    
        #region inspector

        [SerializeField]
        private Button yesButton;
        [SerializeField]
        private Button noButton;
        [SerializeField]
        private Button closeButton;
    
        #endregion
    
        public Observable<Unit> OnYesButtonClick => _onYesButtonClick;

        protected override UniTask OnInitialize(TModel model)
        {
            this.Bind(yesButton, x =>
                {
                    _onYesButtonClick.Execute(Unit.Default); // пришлось сделать так, иначе проблема с порядком подписчиков, а нужен всего лишь факт нажатия на кнопку
                    Apply(true);
                })
                .Bind(noButton, x => Apply(false))
                .Bind(closeButton, x => Apply(false));
        
            _onYesButtonClick.AddTo(LifeTime);

            return UniTask.CompletedTask;
        }

        private void Apply(bool answer)
        {
            Model.result.Execute(answer);
            Close();
        }
    }

    [Serializable]
    public class DialogViewModel : ViewModelBase
    {
        public ReactiveCommand<bool> result = new ();
    }
}
