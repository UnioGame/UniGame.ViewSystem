namespace UniGame.ViewSystem.Runtime.ViewModels
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Context.Runtime.Connections;
    using global::UniGame.Core.Runtime;
    using UniModules.UniGame.ViewSystem.Runtime.Abstract;
    using UniRx;

    [Serializable]
    public class ContextViewModel : IContextViewModel
    {
        private readonly ContextConnection _context = new();
        private readonly ReactiveCommand _closeCommand = new();
        public ILifeTime LifeTime => _context.LifeTime;

        public IReactiveCommand<Unit> CloseCommand => _closeCommand;
        public virtual bool IsDisposeWithModel => true;
        
        public async UniTask InitializeContextAsync(IContext context)
        {
            _context.Reset();
            _context.Connect(context);

            await OnInitializeContext(_context);
        }

        public void Close() => _closeCommand.Execute();
        
        public void Dispose() => _context.Dispose();

        #region private methods
        
        protected virtual UniTask OnInitializeContext(IContext context)
        {
            return UniTask.CompletedTask;
        }

        #endregion

        
    }
}
