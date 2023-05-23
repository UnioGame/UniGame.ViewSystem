namespace UniGame.ViewSystem.Runtime
{
    using System.Collections.Generic;
    using UiSystem.Runtime.Settings;

    public interface IViewReferencesMap
    {
        IReadOnlyList<UiViewReference> this[string view] { get; }
        IReadOnlyList<UiViewReference> Find(string view);
        void Add(UiViewReference reference);
    }
}