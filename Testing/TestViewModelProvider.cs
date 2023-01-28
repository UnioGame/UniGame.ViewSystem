using System;
using UniGame.ViewSystem.Runtime;

namespace Modules.UniModules.UniGame.ViewSystem.Testing
{
    [Serializable]
    public class TestViewModelProvider : ITestViewModelProvider
    {
        public virtual IViewModel Create()
        {
            return new EmptyViewModel();
        }
    }
}