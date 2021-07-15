using System;
using Cysharp.Threading.Tasks;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.UISystem.Runtime.Abstract;
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