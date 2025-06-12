using Cysharp.Threading.Tasks;
using UniGame.ViewSystem.Runtime;

namespace UniModules.UniGame.ViewSystem.Runtime.Abstract
{
    using global::UniGame.Core.Runtime;

    public interface IContextViewModel : 
        IViewModel
    {
        UniTask InitializeContextAsync(IContext context);
    }
}
