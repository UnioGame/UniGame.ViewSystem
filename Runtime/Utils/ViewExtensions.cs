namespace UniModules.UniGame.UISystem.Runtime.Utils
{
    using Abstract;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public static class ViewExtensions
    {
        public static async UniTask<TView> Create<TView>(this IViewProvider source, IViewModel viewModel, Transform parent) 
            where TView : Component, IView
        {
            var view = await source.CreateView<TView>(viewModel);
            view.transform.SetParent(parent, false);

            return view;
        }
    }
}