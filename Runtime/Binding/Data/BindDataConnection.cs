namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;

    [Serializable]
    public struct BindDataConnection
    {
        public IView value;
        public IViewModel source;
        
        public object sourceValue;
        public object targetValue;
    }
}