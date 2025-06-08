namespace UniGame.ViewSystem.Runtime
{
    using System;

    [Serializable]
    public enum ViewStatus : byte
    {
        None,
        Hidden,
        Shown,
        Closed,
        Showing,
        Hiding
    }
}