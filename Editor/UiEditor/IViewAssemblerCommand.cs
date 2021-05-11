using UniGame.UiSystem.Runtime.Settings;
using UniModules.UniGame.Core.Runtime.Interfaces;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{
    public interface IViewAssemblerCommand : ICommand<ViewsSettings, bool>, IResetable
    {
    }
}