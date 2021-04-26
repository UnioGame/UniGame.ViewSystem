using Cysharp.Threading.Tasks;
using UniModules.UniGame.UISystem.Runtime.Abstract;

namespace UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Extensions
{
    public static class ContextViewExtensions
    {
        
        public static IGameViewSystem ActiveView { get; private set; }
        

        public static async UniTask<IView> OpenWindow<TView>(string skinTag = "", string viewName = null)
        {
            return null;
        }

        public static async UniTask<IView> OpenScreen<TView>(string skinTag = "", string viewName = null)
        {
            return null;
        }

        public static async UniTask<IView> OpenOverlay<TView>(string skinTag = "", string viewName = null)
        {
            return null;
        }

        public static IGameViewSystem MakeActive(this IGameViewSystem viewSystem)
        {
            ActiveView = viewSystem;

            var lifeTime = ActiveView.LifeTime;
            lifeTime.AddCleanUpAction(() =>
            {
                if (ActiveView != viewSystem)
                    return;
                ActiveView = null;
            });

            return viewSystem;
        }
        
        public static bool TryMakeActive(this IGameViewSystem viewSystem)
        {
            if (ActiveView != null)
                return false;
            
            MakeActive(viewSystem);
            return true;
        }
        
    }
}
