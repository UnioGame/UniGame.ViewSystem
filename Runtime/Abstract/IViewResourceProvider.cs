using UnityEngine.AddressableAssets;

namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;
    using System.Collections.Generic;
    using Core.Runtime.DataFlow.Interfaces;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Providing views by specified type and skin
    /// </summary>
    /// <typeparam name="TView">Base view type</typeparam>
    public interface IViewResourceProvider
    {
        UniTask<AssetReferenceGameObject> GetViewReferenceAsync(
            Type viewType,
            string skinTag = "",
            bool strongMatching = true,
            string viewName = "");
        
        /// <summary>
        /// Load view by type and target skin tag
        /// </summary>
        /// <returns>found view or null</returns>
        UniTask<TView> LoadViewAsync<TView>(
            Type viewType,
            ILifeTime lifeTime,
            string skinTag = null,
            bool strongMatching = true,
            string viewName = null) where TView : Object;

        /// <summary>
        /// load all Views with target Type
        /// </summary>
        List<UniTask<TView>> LoadViewsAsync<TView>(
            Type viewType,
            ILifeTime lifeTime,
            string skinTag = null,
            bool strongMatching = true) where TView : Object;

    }
}
