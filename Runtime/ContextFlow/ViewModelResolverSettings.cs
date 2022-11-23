using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniGame.UiSystem.Runtime;
using UniGame.Core.Runtime;
using UniGame.ViewSystem.Runtime;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    /// <summary>
    /// Create View Model by requested type 
    /// </summary>
    [Serializable]
    public class ViewModelResolverSettings : IViewModelResolver
    {
        #region inspector

        public int modelFactoryTimeoutMs = 500;
        
        public List<ViewModelResolverVariant> modelResolvers = new List<ViewModelResolverVariant>();
        
        #endregion
        
        private List<IViewModelResolver> _runtimeFactories;

        private IViewModelResolver _contextModelFactory;

        public IEnumerable<IViewModelResolver> Providers => _runtimeFactories;

        public bool IsValid(Type modelType)
        {
            return Providers.Any(x => x.IsValid(modelType));
        }

        public void Initialize()
        {
            _contextModelFactory = new ContextViewModelFactory();
            _runtimeFactories = new List<IViewModelResolver>();
            //register context provider as first
            _runtimeFactories.Add(_contextModelFactory);
            _runtimeFactories.AddRange(modelResolvers.Select(x => x.Value));
        }

        public async UniTask<IViewModel> Create(IContext context,Type type)
        {
            foreach (var modelProvider in _runtimeFactories)
            {
                if(modelProvider == null)
                    continue;
                
                var isValid = modelProvider.IsValid(type);
                if (!isValid) continue;
                var model = await modelProvider.Create(context,type);
                return model;
            }

            return new ViewModelBase();
        }
        
    }
}
