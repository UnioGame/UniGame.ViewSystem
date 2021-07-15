using UniGame.Core.Runtime.ReflectionUtils;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniModules.UniGame.ViewSystem.Runtime.Abstract;
    using UnityEngine;

    [Serializable]
    public class ContextViewModelFactory : IViewModelResolver
    {
        #region inspector

        [SerializeField] private DefaultConstructorViewModelFactory
            _defaultConstructorFactory = new DefaultConstructorViewModelFactory();

        #endregion

        public bool IsValid(Type modelType) => _defaultConstructorFactory.IsValid(modelType) &&
                                               AssignableTypeValidator<IContextViewModel>.Validate(modelType);

        public async UniTask<IViewModel> Create(IContext context, Type type)
        {
            var lifeTime = context.LifeTime;
            var viewModel = await _defaultConstructorFactory.Create(context, type);
            var model = viewModel as IContextViewModel;
            await model.InitializeContextAsync(context)
                .AttachExternalCancellation(lifeTime.AsCancellationToken());
            return model;
        }
    }

    [Serializable]
    public class ContextApiViewModelFactory : IViewModelResolver
    {
        [SerializeField] private ContextViewModelFactory _contextViewModelFactory = new ContextViewModelFactory();

        public bool IsValid(Type modelType)
        {
            var result = AssignableTypeValidator<IContextViewModel>.Validate(modelType);
            return result;
        }

        public UniTask<IViewModel> Create(IContext context, Type modelType)
        {
            return _contextViewModelFactory.Create(context, modelType);
        }
    }
}