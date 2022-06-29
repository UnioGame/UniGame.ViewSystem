namespace Taktika.UI.Common.ViewModels
{
    using UniGame.UiSystem.Runtime;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;

    public class AnimatedClockViewModel : ViewModelBase
    {
        public IReactiveProperty<float> FillPercentage { get; }

        public AnimatedClockViewModel()
        {
            FillPercentage = new ReactiveProperty<float>().AddTo(LifeTime);
        }
    }
}