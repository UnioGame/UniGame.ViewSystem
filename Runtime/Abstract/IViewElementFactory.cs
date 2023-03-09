using UniGame.Core.Runtime;

namespace UniGame.ViewSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IViewElementFactory
    {
        UniTask<IView> Create(
            Type viewType,
            string skinTag = "",
            Transform parent = null,
            string viewName = null,
            bool stayWorldPosition = false,
            ILifeTime ownerLifeTime = null);
        
        UniTask<IView> Create(
            IViewModel viewModel,
            Type viewType,
            string skinTag = "",
            Transform parent = null,
            string viewName = null,
            bool stayWorldPosition = false,
            ILifeTime ownerLifeTime = null);
    }
}