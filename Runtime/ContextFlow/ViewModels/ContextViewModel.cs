using Cysharp.Threading.Tasks;
using UniModules.UniGame.Context.Runtime.Connections;
using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.ViewSystem.Runtime.Abstract;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow.ViewModels
{
    public class ContextViewModel : IContextViewModel
    {
        private ContextConnection _context = new ContextConnection();

        public ILifeTime LifeTime => _context.LifeTime;

        public virtual bool IsDisposeWithModel => true;
        
        public async UniTask InitializeContextAsync(IContext context)
        {
            _context.Reset();
            _context.Connect(context);

            await OnInitializeContext(_context);
        }

        public void Dispose() => _context.Dispose();

        #region private methods
        
        protected virtual async UniTask OnInitializeContext(IContext context)
        {
            
        }

        #endregion
        
    }
}
