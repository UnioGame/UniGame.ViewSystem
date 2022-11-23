namespace UniGame.ViewSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IViewFactory
    {
        UniTask<IView> Create(
            Type viewType, 
            string skinTag = "", 
            Transform parent = null, 
            string viewName = null,
            bool stayWorldPosition = false);
    }
}