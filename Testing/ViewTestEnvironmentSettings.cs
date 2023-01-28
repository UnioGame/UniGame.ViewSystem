using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniGame.UiSystem.Runtime;
using UniGame.UiSystem.Runtime.Settings;
using UnityEngine;

#if UNITY_EDITOR
using UniModules.Editor;
#endif

namespace Modules.UniModules.UniGame.ViewSystem.Testing
{
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Testing/ViewTestEnvironmentSettings")]
    public class ViewTestEnvironmentSettings : ScriptableObject
    {
        [Required]
        public GameViewSystemAsset viewSystem;
        
        [Required]
        public ViewSystemSettings viewSettings;
        
        public string[] viewFolder = {"Assets"};
        
        [BoxGroup("models")]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        public List<ViewTestingData> viewsData = new List<ViewTestingData>();

        [Button]
        public void FillSettings()
        {
            if (viewSettings == null)
                viewSettings = AssetEditorTools.GetAsset<ViewSystemSettings>(viewFolder);
            if (viewSystem == null)
                viewSystem = AssetEditorTools.GetAsset<GameViewSystemAsset>(viewFolder);
        }
    }
}
