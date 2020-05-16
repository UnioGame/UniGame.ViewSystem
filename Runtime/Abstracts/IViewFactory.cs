namespace UniGreenModules.UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using global::UniGame.UiSystem.Runtime.Abstracts;
    using UniRx.Async;
    using UnityEngine;

    public interface IViewFactory
    {
        UniTask<T> Create<T>(string skinTag = "", Transform parent = null) where T : Component, IView;

        UniTask<IView> Create(Type viewType, string skinTag = "", Transform parent = null);
    }
}