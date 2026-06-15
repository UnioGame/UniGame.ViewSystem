using UniCore.Runtime.ProfilerTools;
using UniGame.UiSystem.Runtime.Settings;

namespace UniModules.UniGame.ViewSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::UniGame.ViewSystem.Editor.EditorAssets;
    using UniModules.Editor;
    using global::UniGame.ViewSystem.Runtime;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;

    public class ViewsAssemblyBuilder
    {
        private List<ViewModelResolver> contextViewsMapSettings = new List<ViewModelResolver>();
        private AddressableAssetSettings addressableAssetSettings;
        private List<Action> rebuildCommands;
        private List<IViewAssemblerCommand> rebuildSettingsCommands;

        public ViewsAssemblyBuilder()
        {
            rebuildCommands = new List<Action>()
            {
                Reset,
                RebuildViewSettings,
                UpdateSkins
            };

            rebuildSettingsCommands = new List<IViewAssemblerCommand>()
            {
                new ViewCleanerCommand(),
                new ValidateSettingsCommand(),
                new BuildViewSettingsData(),
            };
        }
        
        public void RebuildAll()
        {
            rebuildCommands.ForEach(x => x.Invoke());
        }

        public void RebuildViewSettings()
        {
            var viewSettings = AssetEditorTools
                .GetAssets<ViewsSettings>()
                .Where(IsProjectViewSettings);

            foreach (var setting in viewSettings)
                Build(setting);

            AssetDatabase.SaveAssets();
        }

        public void Build(ViewsSettings settings)
        {
            if (!settings) {
                GameLog.LogError($"EMPTY UiManagerSettings on UiAssemblyBuilder.Build");
                return;
            }

            if (settings.isActive == false) return;

            addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;

            ApplyViewSettingsPipeline(settings);
            
            settings.MarkDirty();
        }

        public void UpdateSkins()
        {
            ViewSystemEditorAsset.Asset.Validate();
        }

        private HashSet<ViewsSettings> GetSettings(ViewsSettings viewsSettings, HashSet<ViewsSettings> settings)
        {
            if (!settings.Add(viewsSettings))
                return settings;

            if (!(viewsSettings is ViewSystemSettings systemSettings)) return settings;
            
            var nestedSettings = systemSettings.sources;
            foreach (var nestedSetting in nestedSettings)
            {
                GetSettings(nestedSetting.viewSourceReference.editorAsset,settings);
            }

            return settings;
        }
        
        private ViewsSettings ApplyViewSettingsPipeline(ViewsSettings settings)
        {
            foreach (var settingsCommand in rebuildSettingsCommands)
            {
                var result = settingsCommand.Execute(settings);
                if (!result) break;
            }

            return settings;
        }

        private static bool IsProjectViewSettings(ViewsSettings settings)
        {
            if (!settings) return false;

            var path = AssetDatabase.GetAssetPath(settings);
            return path.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase);
        }

        public void Reset()
        {
            rebuildSettingsCommands.ForEach(x => x.Reset());
            contextViewsMapSettings.Clear();
        }

        
    }
}
