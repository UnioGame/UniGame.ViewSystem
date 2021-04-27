using System;
using Cysharp.Threading.Tasks;
using UniModules.UniGame.UISystem.Runtime.Abstract;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    public interface IViewModelProvider
    {
        bool IsValid(Type modelType);

        public UniTask<IViewModel> Create(Type modelType);
    }
}