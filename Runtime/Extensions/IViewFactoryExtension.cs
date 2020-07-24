namespace UniModules.UniGame.UISystem.Runtime.Extensions
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.UiSystem.Runtime.Abstracts;
    using UniGreenModules.UniGame.UiSystem.Runtime.Abstracts;
    using UnityEngine;

    public static class IViewFactoryExtension
    {
        public static async UniTask<T> Create<T>(this IViewFactory factory, string skinTag = "", Transform parent = null, string viewName = null) where T : Component, IView
        {
            var view = await factory.Create(typeof(T), skinTag, parent, viewName) as T;
            return view;
        }
    }
}