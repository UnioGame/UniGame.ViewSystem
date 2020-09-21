namespace UniGame.ModelViewsMap.Runtime.Settings
{
    using System;
    using System.Collections.Generic;
    using UniGreenModules.UniGame.Core.Runtime.SerializableType;

    [Serializable]
    public class ViewTypes
    {
        public SType       Model = new SType();
        public List<SType> Views = new List<SType>();
    }
}