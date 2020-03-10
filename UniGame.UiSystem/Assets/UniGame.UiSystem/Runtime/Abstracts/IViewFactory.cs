namespace UniGreenModules.UniGame.UiSystem.Runtime.Abstracts
{
    using UniRx.Async;
    using UnityEngine;

    public interface IViewFactory
    {
        UniTask<T> Create<T>(string skinTag = "") where T :Component, IView;
        
    }
}