using UniGame.UiSystem.Runtime.Settings;
using UniGame.Core.Runtime;

namespace UniModules.UniGame.ViewSystem
{
    public interface IViewAssemblerCommand : ICommand<ViewsSettings, bool>, IResetable
    {
    }
}