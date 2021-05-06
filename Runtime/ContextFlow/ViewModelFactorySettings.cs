using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniGame.UiSystem.Runtime;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.UISystem.Runtime.Abstract;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    /// <summary>
    /// Create View Model by requested type 
    /// </summary>
    [Serializable]
    public class ViewModelFactorySettings : IViewModelProvider
    {
        #region inspector

        public int modelFactoryTimeoutMs = 500;
        
        public List<ProviderVariant> modelProviders = new List<ProviderVariant>();
        
        #endregion
        
        private List<IViewModelProvider> _runtimeFactories;

        private IViewModelProvider _contextModelFactory;

        public IEnumerable<IViewModelProvider> Providers => _runtimeFactories;

        public bool IsValid(Type modelType)
        {
            return Providers.Any(x => x.IsValid(modelType));
        }

        public void Initialize()
        {
            _contextModelFactory = new ContextViewModelFactory(modelFactoryTimeoutMs);
            _runtimeFactories = new List<IViewModelProvider>();
            //register context provider as first
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
                {
                    var model = await modelProvider.Create(context,type);
                    return model;
                }
            }

            return new ViewModelBase();
        }
        
    }
}
