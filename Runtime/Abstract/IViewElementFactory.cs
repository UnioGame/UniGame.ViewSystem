using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;

namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IViewElementFactory
    {
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