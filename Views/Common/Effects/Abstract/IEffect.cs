namespace Taktika.UI.Common.Effects.Abstract
{
    using System;
    using DG.Tweening;

    public interface IEffect
    {
        IObservable<Sequence> Play();
    }
}