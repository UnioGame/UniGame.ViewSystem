namespace UniGame.ViewSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IViewFactory : IDisposable
    {
        UniTask<IView> Create(
            string viewId, 
            string skinTag = "", 
            Transform parent = null, 
            string viewName = null,
            bool stayWorldPosition = false);
    }
}