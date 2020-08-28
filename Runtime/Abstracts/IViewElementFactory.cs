namespace UniGame.UiSystem.Runtime
{
    using System;
    using Abstracts;
    using Cysharp.Threading.Tasks;
    
    using UnityEngine;

    public interface IViewElementFactory
    {
        UniTask<IView> Create(
            IViewModel viewModel,
            Type viewType,
            string skinTag = "",
            Transform parent = null,
            string viewName = null);
    }
}