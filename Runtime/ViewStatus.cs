namespace UniModules.UniGame.UISystem.Runtime
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