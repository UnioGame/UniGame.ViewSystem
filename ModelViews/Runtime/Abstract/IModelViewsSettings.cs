namespace UniGame.ModelViewsMap.Runtime.Settings
{
    using System;
    using System.Collections.Generic;
    using UniModules.UniGame.Core.Runtime.SerializableType;

    public interface IModelViewsSettings
    {
        IReadOnlyList<SType> this[Type value] { get; }
    }
}