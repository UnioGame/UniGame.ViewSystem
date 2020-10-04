namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    using System;
    using System.Collections.Generic;
    using global::UniGame.Addressables.Reactive;

    /// <summary>
    /// Providing views by specified type and skin
    /// </summary>
    /// <typeparam name="TView">Base view type</typeparam>
    public interface IViewResourceProvider<TView>
    {
        /// <summary>
        /// Load view by type and target skin tag
        /// </summary>
        /// <returns>found view or null</returns>
        IAddressableObservable<TView> LoadViewAsync(
            Type viewType,
            string skinTag = null,
            bool strongMatching = true,
            string viewName = null);

        /// <summary>
        /// load all Views with target Type
        /// </summary>
        List<IAddressableObservable<TView>> LoadViewsAsync(Type viewType,
            string skinTag = null,
            bool strongMatching = true);

    }
}
