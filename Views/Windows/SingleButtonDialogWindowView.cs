using Cysharp.Threading.Tasks;
using UniGame.ViewSystem.Views.Abstract.ViewModels;
using UniGame.UiSystem.Runtime;
using UniGame.Rx.Runtime.Extensions;
using UnityEngine.UI;
using UniGame.Rx.Runtime.Extensions;

public class SingleButtonDialogWindowView : View<IDialogViewModel>
{
    
    #region inspector

    public Button yesButton;

    public Button noButton;

    public Button closeButton;
    
    #endregion

    protected override UniTask OnInitialize(IDialogViewModel model)
    {
        this.Bind(yesButton, x => Apply(true))
            .Bind(noButton, x => Apply(false))
            .Bind(closeButton, x => Apply(false));
        
        return UniTask.CompletedTask;
    }

    public void Apply(bool answer)
    {
        Model.ResultCommand.Execute(answer);
        Close();
    }
}
