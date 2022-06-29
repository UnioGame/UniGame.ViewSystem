namespace Taktika.UI.Common.Views
{
    using Cysharp.Threading.Tasks;
    using Taktika.UI.Common.ViewModels;
    using UniGame.UiSystem.Runtime;
    using UniModules.Rx.Extensions;
    using UnityEngine;
    using UnityEngine.UI;

    public class AnimatedClockView : UiCanvasGroupView<AnimatedClockViewModel>
    {
        [SerializeField] private Image _fillBackground;
        [SerializeField] private Transform _arrow;

        protected override UniTask OnViewInitialize(AnimatedClockViewModel model)
        {
            base.OnViewInitialize(model);

            this.Bind(Model.FillPercentage, UpdateFillPercentage);
            
            return UniTask.CompletedTask;
        }

        private void UpdateFillPercentage(float val)
        {
            _fillBackground.fillAmount = val;
            var currentRotation = _arrow.localEulerAngles; 
            _arrow.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, -val * 360);
        }
    }
}