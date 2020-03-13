namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx.Async;
    using UnityEngine;

    public interface IGameViewSystem : 
        ILifeTimeContext, 
        IViewElementFactory,
        IViewLayoutContainer,
        IDisposable
    {

        UniTask<T> OpenWindow<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView;

        UniTask<T> OpenScreen<T>(IViewModel viewModel, string skinTag = "")
            where T : Component, IView;

        UniTask<T> OpenOverlay<T>(IViewModel viewModel, string skinTag = "") 
            where T : Component, IView;

    }

}