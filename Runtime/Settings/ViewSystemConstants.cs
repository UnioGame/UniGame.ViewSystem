﻿using System;
using UniGame.UiSystem.Runtime;
using UniGame.ViewSystem.Runtime;

namespace UniModules.UniGame.ViewSystem
{
    public static class ViewSystemConstants
    {
        public static Type BaseViewType  = typeof(IUiView<>);
        public static Type BaseModelType = typeof(IViewModel);
    }
}