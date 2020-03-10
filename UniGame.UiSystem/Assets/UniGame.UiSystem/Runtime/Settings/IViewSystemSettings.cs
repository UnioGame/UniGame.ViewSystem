namespace UniGreenModules.UniGame.UiSystem.Runtime.Settings
{
    using System;
    using Abstracts;

    public interface IViewSystemSettings : IDisposable
    {
        IViewResourceProvider UIResourceProvider { get; }
    }
}