﻿using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.AssetReferencies;

namespace UniGame.ModelViewsMap.Runtime.Settings
{
    using System;
    using UiSystem.Runtime;

    [Serializable]
    public class AssetReferenceUiSystem : AssetReferenceComponent<GameViewSystemAsset>
    {
        public AssetReferenceUiSystem(string guid) : base(guid)
        {
        }
    }
}