using UnityEngine;

namespace UniGame.UiSystem.ModelViews.Examples.SimpleUiExample.Scripts
{
    using ModelViewsMap.Runtime.Settings;

    public class SimpleDemoLauncher : MonoBehaviour
    {
        [SerializeField]
        private ModelViewsModuleSettings _settings;

        [SerializeField]
        private DemoScenario _scenario;
        
        // Start is called before the first frame update
        private async void Start()
        {
            Debug.Log("SimpleDemoLauncher Start");
            await _settings.Initialize();
            
            Debug.Log("SimpleDemoLauncher Initialized");
            _scenario.Execute();
        }

    }
}
