using Cysharp.Threading.Tasks;
using UniGame.UiSystem.Runtime;
using UniGame.Runtime.Rx.Runtime.Extensions;
using UnityEngine.UI;
using UniGame.ViewSystem.Views.Windows;

public class SingleButtonDialogWindowView : View<DialogViewModel>
{
    
    #region inspector

    public Button yesButton;

    public Button noButton;

    public Button closeButton;
    
    #endregion

    protected override UniTask OnInitialize(DialogViewModel model)
    {
        this.Bind(yesButton, x => Apply(true))
            .Bind(noButton, x => Apply(false))
            .Bind(closeButton, x => Apply(false));
        
        return UniTask.CompletedTask;
    }

    public void Apply(bool answer)
    {
        Model.result.Execute(answer);
        Close();
    }
}
