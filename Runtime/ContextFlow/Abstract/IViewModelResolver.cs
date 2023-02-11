using System;
using Cysharp.Threading.Tasks;
using UniGame.Core.Runtime;
using UniGame.ViewSystem.Runtime;

namespace UniGame.ViewSystem.Runtime
{
    public interface IViewModelResolver
    {
        bool IsValid(Type modelType);

        public UniTask<IViewModel> Create(IContext context,Type modelType);
    }
}