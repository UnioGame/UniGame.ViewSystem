namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniCore.Runtime.Interfaces;
    using UniRx.Async;
    using UnityEngine;

    public interface IGameViewSystem : ILifeTimeContext, 
        IViewElementFactory, 
        IDisposable
    {
        UniTask<T> OpenWindow<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView;

        UniTask<T> OpenScreen<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView;

        UniTask<T> OpenOverlay<T>(IViewModel viewModel, string skinTag = "") where T : Component, IView;

        T Get<T>() where T : Component, IView;
    }
}