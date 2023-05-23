namespace UniGame.ViewSystem.Runtime
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IViewFactory
    {
        UniTask<IView> Create(
            string viewId, 
            string skinTag = "", 
            Transform parent = null, 
            string viewName = null,
            bool stayWorldPosition = false);
    }
}