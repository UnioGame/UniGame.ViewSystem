using System;
using Cysharp.Threading.Tasks;
using UniGame.Core.Runtime;
using UnityEngine;

namespace UniGame.ViewSystem.Runtime
{
    public abstract class ViewModelResolverAsset : 
        ScriptableObject, 
        IViewModelResolver
    {
        public abstract bool IsValid(Type modelType);
        
        public abstract UniTask<IViewModel> CreateViewModel(IContext context,Type modelType);
    }
}