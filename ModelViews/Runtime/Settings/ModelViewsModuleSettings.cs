using UnityEngine;

namespace UniGame.ModelViewsMap.Runtime.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Taktika.GameRuntime.Types;
    using UiSystem.ModelViews.Runtime.Flow;
    using UiSystem.Runtime;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniGreenModules.UniGame.Core.Runtime.SerializableType;
    using UniModules.UniGame.Context.Runtime.Abstract;
    using UniModules.UniGame.UISystem.Runtime.DataSources;
    

    [CreateAssetMenu(menuName = "UniGame/UiSystem/ModelViewsModuleSettings", fileName = "ModelViewsModuleSettings")]
    public class ModelViewsModuleSettings : AsyncContextDataSource, IModelViewsSettings
    {
        private static List<SType> _emptyList = new List<SType>();

        [Header("is auto rebuild active")]
        public bool isRebuildActive = true;
        
        [Header("Project paths that triggers rebuild on asset changes")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FolderPath]
#endif
        public List<string> updateTargets = new List<string>();
        
        [Space]
        [ReadOnlyValue]
        [SerializeField]
        private ModelViewsTypeMap modelViewsTypeMap = new ModelViewsTypeMap();

        [SerializeField]
        private AssetReferenceUiSystemSource uiViewSystemSource;

        #region public property

        public IReadOnlyList<SType> this[Type value] {
            get {
                modelViewsTypeMap.TryGetValue(value, out var views);
                return views?.Views ?? _emptyList;
            }
        }

        #endregion
        
        #region public methods

        public async UniTask<IGameViewSystem> Initialize()
        {
            var source = await uiViewSystemSource.LoadAssetTaskAsync(LifeTime);
            
            var uiSystem = await source.LoadSystem();

            ModelsViewsFlow.Initialize(uiSystem,this);
            
            return uiSystem;
        }
        
        public override async UniTask<IContext> RegisterAsync(IContext context)
        {
            var uiSystem = await Initialize();
            context.Publish(uiSystem);
            return context;
        }

        public IReadOnlyList<IReadOnlyType> UpdateValue(Type modelKey, IReadOnlyList<Type> viewTypes)
        {
            var types = viewTypes.
                         Select(x => (SType) x).
                         ToList();
            
            var viewSerializedTypes = new ViewTypes() {
                Views = new List<SType>(types),
                Model = modelKey,
            };
            
            modelViewsTypeMap[modelKey] = viewSerializedTypes;
            
            return viewSerializedTypes.Views;
        }
        
        public void CleanUp()
        {
            modelViewsTypeMap.Clear();
        }
        
        #endregion
        
        
        #region Editor methods

#if UNITY_EDITOR

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("Reset")]
#endif
        private void EditorReset()
        {
            isRebuildActive = false;
            modelViewsTypeMap.Clear();
            uiViewSystemSource = new AssetReferenceUiSystemSource(Guid.Empty.ToString());
            updateTargets.Clear();
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
#endif
        
        
        #endregion
        
   }
}
