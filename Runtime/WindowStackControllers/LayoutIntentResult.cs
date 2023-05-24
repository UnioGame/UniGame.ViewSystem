namespace UniGame.UiSystem.Runtime
{
    using System;
    using ViewSystem.Runtime;

    [Serializable]
    public struct LayoutIntentResult
    {
        public bool stopPropagation;
        public IView view;
    }
}