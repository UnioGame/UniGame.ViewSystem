namespace UniGame.ViewSystem.Views.Windows
{
    using System;
    using Cysharp.Threading.Tasks;
    using Abstract.ViewModels;
    using UniGame.Rx.Runtime.Extensions;
    using UniGame.UiSystem.Runtime;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class DialogView<TModel> : WindowView<TModel>
        where TModel : class, IDialogViewModel
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
    
        public IObservable<Unit> OnYesButtonClick => _onYesButtonClick;

        protected override UniTask OnViewInitialize(TModel model)
        {
            this.Bind(yesButton, x =>
                {
                    _onYesButtonClick.Execute(); // пришлось сделать так, иначе проблема с порядком подписчиков, а нужен всего лишь факт нажатия на кнопку
                    Apply(true);
                })
                .Bind(noButton, x => Apply(false))
                .Bind(closeButton, x => Apply(false));
        
            _onYesButtonClick.AddTo(LifeTime);

            return UniTask.CompletedTask;
        }

        private void Apply(bool answer)
        {
            Model.ResultCommand.Execute(answer);
            Close();
        }
    }

    [Serializable]
    public class DialogViewModel : ViewModelBase,IDialogViewModel
    {
        public IReactiveCommand<bool> resultCommand = new ReactiveCommand<bool>();

        public IReactiveCommand<bool> ResultCommand => resultCommand;
    }
}
