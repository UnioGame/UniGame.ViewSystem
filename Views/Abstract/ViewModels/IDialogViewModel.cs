using UniModules.UniGame.UISystem.Runtime.Abstract;
using UniRx;

public interface IDialogViewModel : IViewModel
{
    IReactiveCommand<bool> ResultCommand { get; }
    string                 ViewModelId   { get; }
}