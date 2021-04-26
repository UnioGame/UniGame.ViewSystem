using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniModules.UniGame.UISystem.Runtime.Abstract;
using UnityEngine;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Settings/ViewContextMap",fileName = nameof(ViewContextMapSettings))]
    public class ViewContextMapSettings : ScriptableObject,IViewModelProvider
    {
        #region inspector
        
        [SerializeReference]
        public List<ProviderVariant> modelProviders = new List<ProviderVariant>();

        #endregion
        
        public IEnumerable<IViewModelProvider> Providers => modelProviders.Select(x => x.Value);

        public bool IsValid(Type modelType)
        {
            return Providers.Any(x => x.IsValid(modelType));
        }

        public async UniTask<IViewModel> Create(Type type)
        {
            foreach (var modelProviderValue in modelProviders)
            {
                var modelProvider = modelProviderValue.value;
                if(modelProvider == null)
                    continue;
                
                var isValid = modelProvider.IsValid(type);
                if (isValid)
                    return await modelProvider.Create(type);
            }

            return null;
        }
    }
}
