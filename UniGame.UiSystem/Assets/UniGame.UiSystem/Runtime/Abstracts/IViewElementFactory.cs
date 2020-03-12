namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UniRx.Async;
    using UnityEngine;

    public interface IViewElementFactory
    {
        UniTask<T> Create<T>(IViewModel viewModel, string skinTag = "", Transform parent = null) 
            where T :Component, IView;

        UniTask<T> OpenWindow<T>(IViewModel viewModel,string skinTag = "") 
            where T :Component, IView;

        UniTask<T> OpenScreen<T>(IViewModel viewModel,string skinTag = "") 
            where T :Component, IView;
    }
}