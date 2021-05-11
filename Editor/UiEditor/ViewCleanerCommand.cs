using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{
    public class ViewCleanerCommand  :IViewAssemblerCommand
    {
        public bool Execute(ViewsSettings value)
        {
            return true;
        }

        public void Reset()
        {
            
        }
    }
}