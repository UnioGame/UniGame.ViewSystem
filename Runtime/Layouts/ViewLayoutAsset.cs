namespace UniGame.LeoEcs.ViewSystem.Layouts.Converters
{
    
    using UniGame.ViewSystem.Runtime.WindowStackControllers.Abstract;
    using UnityEngine;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Layout/Custom Layout", fileName = nameof(ViewLayoutAsset))]

    public class ViewLayoutAsset : ScriptableObject
    {
        public string layoutId;

#if ODIN_INSPECTOR
        [InlineEditor]
#endif
        public ViewLayoutType layout;
    }
}