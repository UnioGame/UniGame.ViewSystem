
namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    
    [Serializable]
    public class DefaultConstructorViewModelFactory : IViewModelProvider
    {
        public bool IsValid(Type modelType)
        {
            var result = !modelType.IsAbstract && !modelType.IsInterface && modelType.HasDefaultConstructor();
            return result;
        }

        public async UniTask<IViewModel> Create(IContext context, Type modelType)
        {
            var model = modelType.CreateWithDefaultConstructor<IViewModel>();
            return model;
        }
    }
}