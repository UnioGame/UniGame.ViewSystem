﻿using UniModules.UniGame.UISystem.Runtime;

namespace UniGame.ViewSystem.Runtime
{
    using UniRx;

    public interface IViewStatus
    {
        IReadOnlyReactiveProperty<ViewStatus> Status { get; }
    }
}