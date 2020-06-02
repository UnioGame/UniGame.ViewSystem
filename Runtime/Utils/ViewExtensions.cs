namespace UniModules.UniGame.UISystem.Runtime.Utils
{
    using Abstracts;
    using global::UniGame.UiSystem.Runtime.Abstracts;
    using UniRx.Async;
    using UnityEngine;

    public static class ViewExtensions
    {
        public static async UniTask Create<TView>(this IViewProvider source, IViewModel viewModel, Transform parent) 
            where TView : Component, IView
        {
            var view = await source.CreateView<TView>(viewModel);
            view.transform.SetParent(parent, false);
        }
    }
}