using System;
using Cysharp.Threading.Tasks;
using UniGame.Core.Runtime;

namespace UniGame.ViewSystem.Runtime
{
    public interface IViewModelResolver
    {
        bool IsValid(Type modelType);

        public UniTask<IViewModel> CreateViewModel(IContext context,Type modelType);

        
    }
}