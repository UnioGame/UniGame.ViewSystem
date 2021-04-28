using Cysharp.Threading.Tasks;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.UISystem.Runtime.Abstract;

namespace UniModules.UniGame.ViewSystem.Runtime.Abstract
{
    public interface IContextViewModel : 
        IViewModel
    {
        UniTask InitializeContextAsync(IContext context);
    }
}
