using System;
using Cysharp.Threading.Tasks;
using UniGame.Core.Runtime;
using UniGame.ViewSystem.Runtime;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    public interface IViewModelResolver
    {
        bool IsValid(Type modelType);

        public UniTask<IViewModel> Create(IContext context,Type modelType);
    }
}