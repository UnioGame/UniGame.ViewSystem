namespace UniGame.ViewSystem.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    /// <summary>
    /// Create View Model by requested type 
    /// </summary>
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Settings/View Model Resolver Settings",
        fileName = "View Model Resolver Settings")]
    public class ViewModelResolverSettings : ScriptableObject, IViewModelResolver
    {
        [SerializeField]
#if ODIN_INSPECTOR
        [InlineProperty]
        [HideLabel]
        [GUIColor(0.5f,0.8f,0.4f)]
#endif
        public ViewModelResolver resolver = new();

        public bool IsValid(Type modelType) => resolver.IsValid(modelType);

        public UniTask<IViewModel> CreateViewModel(IContext context, Type modelType) => resolver.CreateViewModel(context, modelType);
    }
}