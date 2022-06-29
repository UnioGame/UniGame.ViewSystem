namespace Taktika.UI.ViewModels
{
    using Abstract;
    using UniGame.UiSystem.Runtime;
    using UnityEngine;

    public class NoInternetTryAgainViewModel : ViewModelBase, INoInternetViewModel
    {
        public void TryAgain()
        {
            Debug.Log($"{nameof(NoInternetTryAgainViewModel)} :: TryAgain()");
        }

        public void Cancel()
        {
           
        }
    }
}