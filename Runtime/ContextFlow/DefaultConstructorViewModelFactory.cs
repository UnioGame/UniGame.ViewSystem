
namespace UniGame.ViewSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Utils;
    using global::UniGame.Core.Runtime;
    using global::UniGame.ViewSystem.Runtime;
    
    [Serializable]
    public class DefaultConstructorViewModelFactory : IViewModelResolver
    {
        public bool IsValid(Type modelType)
        {
            var result = !modelType.IsAbstract && 
                         !modelType.IsInterface && 
                         modelType.HasDefaultConstructor();
            return result;
        }

        public async UniTask<IViewModel> CreateViewModel(IContext context, Type modelType)
        {
            var model = modelType.CreateWithDefaultConstructor<IViewModel>();
            return model;
        }
    }
}