using UniGame.ViewSystem.Runtime;
using UniRx;

public interface IDialogViewModel : IViewModel
{
    IReactiveCommand<bool> ResultCommand { get; }
    string                 ViewModelId   { get; }
}