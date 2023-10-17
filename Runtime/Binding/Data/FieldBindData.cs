namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Reflection;

    [Serializable]
    public struct FieldBindData
    {
        public object source;
        public object target;

        public FieldInfo sourceField;
        public FieldInfo targetField;
        
        public object sourceValue;
        public object targetValue;
    }
}