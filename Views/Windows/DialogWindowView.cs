using System;
using Cysharp.Threading.Tasks;
using UniGame.UiSystem.Runtime;
using UniModules.Rx.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindowView : WindowView<IDialogViewModel>
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

    protected override UniTask OnViewInitialize(IDialogViewModel model)
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
