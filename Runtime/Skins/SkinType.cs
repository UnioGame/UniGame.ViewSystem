namespace Game.Modules.UniModules.UniGame.ViewSystem.Runtime.Skins
{
    using System;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
#if ODIN_INSPECTOR
    [HideLabel]
    [InlineProperty]
    [ValueDropdown("@UniGame.ViewSystem.Editor.EditorAssets.ViewSystemEditorAsset.GetSkins()",IsUniqueList = true)]
#endif
    public class SkinType
    {
        public string view;
    }
}