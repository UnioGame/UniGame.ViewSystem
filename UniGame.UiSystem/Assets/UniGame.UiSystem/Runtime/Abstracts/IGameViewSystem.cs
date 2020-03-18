﻿namespace UniGame.UiSystem.Runtime
{
    using System;

    public interface IGameViewSystem : 
        IDisposable, 
        IViewProvider
    {
        void CloseAll();
    }

}