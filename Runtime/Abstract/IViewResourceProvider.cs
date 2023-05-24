using UnityEngine.AddressableAssets;

namespace UniGame.ViewSystem.Runtime
{
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UiSystem.Runtime.Settings;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Providing views by specified type and skin
    /// </summary>
    /// <typeparam name="TView">Base view type</typeparam>
    public interface IViewResourceProvider
    {
        UniTask<UiViewReference> GetViewReferenceAsync(
            string viewType,
            string skinTag = "",
            string viewName = "");
        
        /// <summary>
        /// Load view by type and target skin tag
        /// </summary>
        /// <returns>found view or null</returns>
        UniTask<TView> LoadViewAsync<TView>(
            string viewType,
            ILifeTime lifeTime,
            string skinTag = "",
            string viewName = "") where TView : Object;

        /// <summary>
        /// load all Views with target Type
        /// </summary>
        // List<UniTask<TView>> LoadViewsAsync<TView>(
        //     string viewType,
        //     ILifeTime lifeTime,
        //     string skinTag = "") where TView : Object;

    }
}
