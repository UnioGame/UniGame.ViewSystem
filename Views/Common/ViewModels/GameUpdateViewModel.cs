namespace Taktika.UI.Common.ViewModels
{
    using UniGame.UiSystem.Runtime;
    using UnityEngine;

    public class GameUpdateViewModel : ViewModelBase
    {
        private readonly string _marketURL = $"market://details?id={Application.identifier}";

        public void OpenStore()
        {
            Application.OpenURL(_marketURL);
        }
    }
}