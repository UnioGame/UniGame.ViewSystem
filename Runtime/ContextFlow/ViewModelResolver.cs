using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniGame.UiSystem.Runtime;
using UniGame.Core.Runtime;
using UniGame.ViewSystem.Runtime;

namespace UniGame.ViewSystem.Runtime
{
    /// <summary>
    /// Create View Model by requested type 
    /// </summary>
    [Serializable]
    public class ViewModelResolver : IViewModelResolver
    {
        #region inspector

        public int modelFactoryTimeoutMs = 500;
        
        public List<ViewModelResolverVariant> modelResolvers = new();
        
        #endregion

        private bool _isInitialized = false;
        
        private List<IViewModelResolver> _runtimeFactories;

        public IEnumerable<IViewModelResolver> Providers => _runtimeFactories;

        public bool IsInitialized => _isInitialized;

        public bool IsValid(Type modelType)
        {
            return Providers.Any(x => x.IsValid(modelType));
        }

        public void Initialize()
        {
            _runtimeFactories = new List<IViewModelResolver>();
            
            //register context provider as first
            _runtimeFactories.AddRange(modelResolvers
                .Where(x => x.Value!=null)
                .Select(x => x.Value));

            _runtimeFactories.Add(new ContextViewModelFactory());
            _runtimeFactories.Add(new DefaultConstructorViewModelFactory());
            
            _isInitialized = true;
        }

        public async UniTask<IViewModel> CreateViewModel(IContext context,Type type)
        {
            foreach (var modelProvider in _runtimeFactories)
            {
                if(modelProvider == null)
                    continue;
                
                var isValid = modelProvider.IsValid(type);
                if (!isValid) continue;
                var model = await modelProvider.CreateViewModel(context,type);
                return model;
            }

            return new ViewModelBase();
        }
        
    }
}
