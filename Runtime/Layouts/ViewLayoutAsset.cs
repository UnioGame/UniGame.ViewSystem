namespace UniGame.LeoEcs.ViewSystem.Layouts.Converters
{
    using Sirenix.OdinInspector;
    using UniModules.UniGame.UISystem.Runtime.WindowStackControllers.Abstract;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Layout/Custom Layout", fileName = nameof(ViewLayoutAsset))]

    public class ViewLayoutAsset : ScriptableObject
    {
        public string layoutId;

        [InlineEditor]
        public ViewLayoutType layout;
    }
}