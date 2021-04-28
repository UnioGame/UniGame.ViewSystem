using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.UISystem.Runtime.Abstract;
using UnityEngine;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    /// <summary>
    /// Create View Model by requested type 
    /// </summary>
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Settings/ViewModelFactorySettings",fileName = nameof(ViewModelFactorySettings))]
    public class ViewModelFactorySettings : ScriptableObject,IViewModelProvider
    {
        #region inspector

        public int modelFactoryTimeoutMs = 500;
        
        public List<ProviderVariant> modelProviders = new List<ProviderVariant>();

        #endregion

        private IViewModelProvider _contextModelFactory;

        private List<IViewModelProvider> _runtimeFactories;
        
        public IEnumerable<IViewModelProvider> Providers => modelProviders.Select(x => x.Value);

        public bool IsValid(Type modelType)
        {
            return Providers.Any(x => x.IsValid(modelType));
        }

        public void Initialize()
        {
            _contextModelFactory = new ContextViewModelFactory(modelFactoryTimeoutMs);
            _runtimeFactories = new List<IViewModelProvider>();
            _runtimeFactories.Add(_contextModelFactory);
            _runtimeFactories.AddRange(modelProviders.Select(x => x.Value));
        }

        public async UniTask<IViewModel> Create(IContext context,Type type)
        {
            foreach (var modelProvider in _runtimeFactories)
            {
                if(modelProvider == null)
                    continue;
                
                var isValid = modelProvider.IsValid(type);
                if (isValid)
                    return await modelProvider.Create(context,type);
            }

            return null;
        }

        
    }
}
