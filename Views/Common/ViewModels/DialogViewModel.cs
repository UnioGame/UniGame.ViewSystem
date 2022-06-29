namespace Taktika.UI.Common.ViewModels
{
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using System;
    using UniGame.UiSystem.Runtime;
    using UniRx;

    [Serializable]
    public class DialogViewModel : ViewModelBase, IDialogViewModel
    {
        private ReactiveCommand<bool> _dialogCommand;

        public IReactiveCommand<bool> ResultCommand => _dialogCommand;

        public string ViewModelId { get; }

        public DialogViewModel(string viewModelId)
        {
            _dialogCommand = new ReactiveCommand<bool>().AddTo(LifeTime);
            ViewModelId    = viewModelId;
        }
    }
}
