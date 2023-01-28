using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem
{
    public class ValidateSettingsCommand : IViewAssemblerCommand
    {
        public bool Execute(ViewsSettings value)
        {
            return value.IsActive;
        }
        
        public void Reset()
        {
            
        }
    }
}