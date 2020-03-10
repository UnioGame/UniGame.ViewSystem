namespace UniGreenModules.UniGame.UiSystem.Runtime
{
    using Abstracts;
    using UniRx.Async;
    using UnityEngine;

    public interface IViewElementFactory
    {
        UniTask<T> Create<T>(IViewModel viewModel,string skinTag = "") 
            where T :Component, IView;

        UniTask<T> CreateWindow<T>(IViewModel viewModel,string skinTag = "") 
            where T :Component, IView;

        UniTask<T> CreateScreen<T>(IViewModel viewModel,string skinTag = "") 
            where T :Component, IView;
    }
}