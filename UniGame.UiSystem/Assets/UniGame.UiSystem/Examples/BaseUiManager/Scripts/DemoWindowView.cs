namespace UniGame.UiSystem.Examples.BaseUiManager
{
    using System.Collections;
    using Runtime;
    using Runtime.Abstracts;
    using UnityEngine;

    public class DemoWindowView : WindowView<IViewModel>
    {
        public float showTime = 2f;

        public float closeTime = 2f;
        
        protected override IEnumerator OnShowProgress()
        {
            var time = 0f;
            
            while (canvasGroup.alpha > 0) {
                canvasGroup.alpha =  Mathf.Lerp(0, showTime, time);
                time              += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 0;
        }
        
        protected override IEnumerator OnHidingProgress()
        {
            var time = 0f;
            
            while (canvasGroup.alpha > 0) {
                canvasGroup.alpha = Mathf.Lerp(0, closeTime, time);
                time += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 0;
            
        }
        
    }
}
