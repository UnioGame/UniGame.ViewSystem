namespace Taktika.UI.Common.MagneticViews.Abstract
{
    using Taktika.Lobby.Runtime.UI.Views;

    public interface IBarkFollowable : IFollowable
    {
        void SetBarkUi(MagneticBarkUiView barkView);
    }
}