namespace UniModules.UniGame.UISystem.Runtime.Utils
{
    using Abstract;
    using Cysharp.Threading.Tasks;
    using Extensions;
    using UniCore.Runtime.Extension;
    using UniRx;
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

        public static async UniTask AwaitIsReadyAsync(this IView view)
        {
            if (view == null || view.ViewLifeTime.IsTerminated)
                return;
            await view.IsInitialized
                .Where(x => x)
                .AwaitFirstAsync(view.ViewLifeTime);
        }

        public static async UniTask AwaitStatusAsync(this IView view,ViewStatus status)
        {
            if (view == null || view.ViewLifeTime.IsTerminated)
                return;
            await view.Status
                .Where(x => x == status)
                .AwaitFirstAsync(view.ViewLifeTime);
        }
        
    }
}