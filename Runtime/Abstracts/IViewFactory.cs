namespace UniGreenModules.UniGame.UiSystem.Runtime.Abstracts
{
    using System;
    using Cysharp.Threading.Tasks;
    using global::UniGame.UiSystem.Runtime.Abstracts;
    
    using UnityEngine;

    public interface IViewFactory
    {
        UniTask<IView> Create(Type viewType, string skinTag = "", Transform parent = null, string viewName = null,
            bool stayWorldPosition = false);
    }
}