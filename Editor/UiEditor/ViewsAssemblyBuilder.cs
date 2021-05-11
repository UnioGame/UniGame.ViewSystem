using UniCore.Runtime.ProfilerTools;
using UniGame.UiSystem.Runtime.Settings;
using UniModules.UniCore.EditorTools.Editor.Utility;
using UniModules.UniCore.Runtime.ReflectionUtils;
using UniModules.UniCore.Runtime.Utils;

namespace UniModules.UniGame.ViewSystem.Editor.UiEditor
{
    using System;
    using System.Collections.Generic;
    using UniModules.UniGame.Core.EditorTools.Editor.AssetOperations;
    using Runtime.ContextFlow;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;

    public static class ViewModelsAssemblyMap
    {
        private static readonly List<Type> _empty = new List<Type>(0);
        
        public static readonly MemorizeItem<Type, IReadOnlyList<Type>> ModelsRealizationMap =
            MemorizeTool.Memorize<Type, IReadOnlyList<Type>>(type =>
            {
                if (!type.IsAbstract && !type.IsInterface)
                {
                    return _empty;
                }
                
                return type.GetAssignableTypes();
            });

        public static void Initialize()
        {
            Build();
        }
        
        public static void Build()
        {
            var viewType = ViewSystemConstants.BaseViewType;
            var modelType = ViewSystemConstants.BaseModelType;

            var allModelTypes = modelType.GetAssignableTypes(false);
            allModelTypes.ForEach(x => ModelsRealizationMap.GetValue(x));
            
        }

    }

    public class ViewsAssemblyBuilder
    {
        private List<ViewModelFactorySettings> contextViewsMapSettings = new List<ViewModelFactorySettings>();
        private AddressableAssetSettings addressableAssetSettings;
        private List<Action> rebuildCommands;
        private List<IViewAssemblerCommand> rebuildSettingsCommands;

        public ViewsAssemblyBuilder()
        {
            rebuildCommands = new List<Action>()
            {
                Reset,
                RebuildViewSettings,
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
            var viewSettings = AssetEditorTools.GetAssets<ViewsSettings>();
            foreach (var setting in viewSettings)
            {
                Build(setting);
                setting.MarkDirty();
            }
        }

        public void Build(ViewsSettings settings)
        {
            addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
            
            if (settings.isActive == false) return;

            if (!settings) {
                GameLog.LogError($"EMPTY UiManagerSettings on UiAssemblyBuilder.Build");
                return;
            }

            ApplyViewSettingsPipeline(settings);
            
            settings.MarkDirty();
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

        public void Reset()
        {
            rebuildSettingsCommands.ForEach(x => x.Reset());
            contextViewsMapSettings.Clear();
        }

        
    }
}