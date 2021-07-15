using System;
using Cysharp.Threading.Tasks;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.UISystem.Runtime.Abstract;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    public interface IViewModelResolver
    {
        bool IsValid(Type modelType);

        public UniTask<IViewModel> Create(IContext context,Type modelType);
    }
}