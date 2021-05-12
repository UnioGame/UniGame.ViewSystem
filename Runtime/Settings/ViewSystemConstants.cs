using System;
using UniGame.UiSystem.Runtime;
using UniModules.UniGame.UISystem.Runtime.Abstract;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{
    public static class ViewSystemConstants
    {
        public static Type BaseViewType  = typeof(IUiView<>);
        public static Type BaseModelType = typeof(IViewModel);
    }
}