using System.Collections.Generic;
using UniModules.UniGame.Core.Editor.EditorProcessors;

namespace UniGame.ViewSystem.Editor.EditorAssets
{
    using System.Linq;
    using UiSystem.Runtime.Settings;
    using UniModules.UniGame.Core.EditorTools.Editor.AssetOperations;

    [GeneratedAssetInfo(location:"ViewSystem/Editor")]
    public class ViewSystemEditorAsset : GeneratedAsset<ViewSystemEditorAsset>
    {

        #region static data

        public static IEnumerable<SkinId> GetSkins() => Asset.viewSkins;
        
        #endregion
        
        public List<SkinId> viewSkins = new List<SkinId>();

        public ViewSystemEditorAsset AddSkin(SkinId skin)
        {
            if (viewSkins.Contains(skin)) return this;
            viewSkins.Add(skin);
            return this;
        }

        public void Validate()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            var viewSettings = AssetEditorTools.GetAssets<ViewsSettings>();
            
            viewSkins.Clear();
            
            foreach (var skin in viewSettings
                .SelectMany(x => x.Views)
                .Select(x => x.Tag)
                .Distinct())
            {
                AddSkin(skin);
            }
        }
    }
}
