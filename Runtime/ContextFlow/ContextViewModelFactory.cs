using System;
using Cysharp.Threading.Tasks;
using UniModules.UniCore.Runtime.Utils;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.UISystem.Runtime.Abstract;
using UniModules.UniGame.ViewSystem.Runtime.Abstract;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    [Serializable]
    public class ContextViewModelFactory : IViewModelProvider
    {
        private static MemorizeItem<Type, bool> isContextTypeMethod = MemorizeTool.Memorize<Type, bool>(x => ContextViewModelAPIType.IsAssignableFrom(x));
        
        public readonly static Type ContextViewModelAPIType = typeof(IContextViewModel);

        public TimeSpan timeOut;
        
        public ContextViewModelFactory(int timeout)
        {
            timeOut = TimeSpan.FromMilliseconds(timeout);
        }
        
        public bool IsValid(Type modelType) => isContextTypeMethod[modelType];

        public async UniTask<IViewModel> Create(IContext context,Type type)
        {
            var model = type.CreateWithDefaultConstructor<IContextViewModel>();
            await model.InitializeContextAsync(context).Timeout(timeOut);
            return model;
        }
    }
}