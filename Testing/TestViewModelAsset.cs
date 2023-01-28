using UniGame.ViewSystem.Runtime;
using UnityEngine;

namespace Modules.UniModules.UniGame.ViewSystem.Testing
{
    public class TestViewModelAsset : ScriptableObject, ITestViewModelProvider
    {
        public virtual IViewModel Create()
        {
            return new EmptyViewModel();
        }
    }
}