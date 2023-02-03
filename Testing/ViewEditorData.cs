using System;
using UniGame.UiSystem.Runtime;
using UnityEngine;

namespace Modules.UniModules.UniGame.ViewSystem.Testing.Editor
{
    [Serializable]
    public class ViewEditorData
    {
        public ViewTestEnvironmentSettings settings;
        public GameViewSystemAsset viewSystem;
        public Canvas canvas;
    }
}