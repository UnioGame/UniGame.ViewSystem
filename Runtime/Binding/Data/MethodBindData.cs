namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Reflection;

    [Serializable]
    public struct MethodBindData
    {
        public object source;
        public object target;

        public object sourceValue;
        public FieldInfo sourceField;
        public MethodInfo method;
    }
}