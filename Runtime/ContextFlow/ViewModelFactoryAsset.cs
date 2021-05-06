using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.Core.Runtime.ScriptableObjects;
using UniModules.UniGame.UISystem.Runtime.Abstract;
using UnityEngine;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    /// <summary>
    /// Create View Model by requested type 
    /// </summary>
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Settings/ViewModelFactorySettings",fileName = nameof(ViewModelFactoryAsset))]
    public class ViewModelFactoryAsset : LifetimeScriptableObject,IViewModelProvider
    {
        #region inspector

        public List<ProviderVariant> modelProviders = new List<ProviderVariant>();
        
        #endregion
        
        private IViewModelProvider _contextModelFactory;

        public IEnumerable<IViewModelProvider> Providers => modelProviders.Select(x => x.Value);

        public bool IsValid(Type modelType)
        {
            return Providers.Any(x => x.IsValid(modelType));
        }

        public async UniTask<IViewModel> Create(IContext context,Type type)
        {
            foreach (var modelProvider in modelProviders)
            {
                var value = modelProvider?.Value;
                
                if(modelProvider == null || value == null)
                    continue;
                
                var isValid = value.IsValid(type);
                if (isValid)
                    return await value.Create(context,type);
            }

            return null;
        }

    }
}
