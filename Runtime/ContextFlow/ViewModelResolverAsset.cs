using System;
using Cysharp.Threading.Tasks;
using UniGame.Core.Runtime;
using UniGame.ViewSystem.Runtime;
using UnityEngine;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    public abstract class ViewModelResolverAsset : 
        ScriptableObject, 
        IViewModelResolver
    {
        public abstract bool IsValid(Type modelType);
        
        public abstract UniTask<IViewModel> Create(IContext context,Type modelType);
    }
}