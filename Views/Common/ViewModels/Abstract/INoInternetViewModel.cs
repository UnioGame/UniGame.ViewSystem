namespace Taktika.UI.ViewModels.Abstract
{
    using UniModules.UniGame.UISystem.Runtime.Abstract;

    public interface INoInternetViewModel : IViewModel
    {
        void TryAgain();
        void Cancel();
    }
}