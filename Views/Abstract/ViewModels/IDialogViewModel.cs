using UniGame.ViewSystem.Runtime;
using UniRx;

namespace UniGame.ViewSystem.Views.Abstract.ViewModels
{
    public interface IDialogViewModel : IViewModel
    {
        IReactiveCommand<bool> ResultCommand { get; }
    }
}