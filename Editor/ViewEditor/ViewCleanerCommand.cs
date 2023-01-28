using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem
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