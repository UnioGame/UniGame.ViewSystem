using Cysharp.Threading.Tasks;
using UniGame.Core.Runtime;
using UniGame.ViewSystem.Runtime;

namespace UniModules.UniGame.ViewSystem.Runtime.Abstract
{
    public interface IContextViewModel : 
        IViewModel
    {
        UniTask InitializeContextAsync(IContext context);
    }
}
