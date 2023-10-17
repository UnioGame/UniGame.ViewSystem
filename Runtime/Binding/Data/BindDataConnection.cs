namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Reflection;
    using UnityEngine.Serialization;

    [Serializable]
    public struct BindDataConnection
    {
        public IView value;
        public IViewModel source;
        
        public object sourceValue;
        public object targetValue;
    }
}