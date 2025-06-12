using UniGame.Core.Runtime.ReflectionUtils;

namespace UniGame.ViewSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using global::UniGame.Core.Runtime;
    using UniModules.UniGame.ViewSystem.Runtime.Abstract;
    using UnityEngine;

    [Serializable]
    public class ContextViewModelFactory : IViewModelResolver
    {
        #region inspector

        [SerializeField] private DefaultConstructorViewModelFactory
            _defaultConstructorFactory = new();

        #endregion

        public bool IsValid(Type modelType) => _defaultConstructorFactory.IsValid(modelType) &&
                                               AssignableTypeValidator<IContextViewModel>.Validate(modelType);

        public async UniTask<IViewModel> CreateViewModel(IContext context, Type type)
        {
            var lifeTime = context.LifeTime;
            var viewModel = await _defaultConstructorFactory.CreateViewModel(context, type);
            var model = viewModel as IContextViewModel;
            await model.InitializeContextAsync(context)
                .AttachExternalCancellation(lifeTime.AsCancellationToken());
            return model;
        }
    }

    [Serializable]
    public class ContextApiViewModelFactory : IViewModelResolver
    {
        [SerializeField] 
        private ContextViewModelFactory _contextViewModelFactory = new();

        public bool IsValid(Type modelType)
        {
            var result = AssignableTypeValidator<IContextViewModel>.Validate(modelType);
            return result;
        }

        public UniTask<IViewModel> CreateViewModel(IContext context, Type modelType)
        {
            return _contextViewModelFactory.CreateViewModel(context, modelType);
        }
    }
}