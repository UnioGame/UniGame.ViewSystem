namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniRx.Async;
    using UnityEngine;

    public interface IViewElementFactory
    {
        UniTask<T> Create<T>(
            IViewModel viewModel, 
            string skinTag = "", 
            Transform parent = null) 
            where T :class, IView;

        UniTask<IView> Create(
            IViewModel viewModel,
            Type viewType,
            string skinTag = "",
            Transform parent = null);
    }
}