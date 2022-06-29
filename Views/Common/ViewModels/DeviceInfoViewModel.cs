namespace Taktika.UI.Common.ViewModels
{
    using UniGame.UiSystem.Runtime;

    public class DeviceInfoViewModel : ViewModelBase
    {
        public readonly string UserId;
        
        public DeviceInfoViewModel(string userId)
        {
            UserId = userId;
        }
    }
}