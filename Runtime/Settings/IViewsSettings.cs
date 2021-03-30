using System.Collections.Generic;

namespace UniGame.UiSystem.Runtime.Settings
{
    public interface IViewsSettings
    {
        IReadOnlyList<UiViewReference> Views { get; }
    }
}