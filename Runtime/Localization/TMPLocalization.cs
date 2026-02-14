namespace UniGame.Localization.Runtime.Components
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.Runtime.DataFlow;
    using R3;
    using TMPro;
    using Runtime;
    using UnityEngine;
    using UnityEngine.Localization;
    using ViewSystem.Runtime;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    public class TMPLocalization : MonoBehaviour
    {
        public TMP_Text textUGUI;
        public LocalizedString localization;
        
        private LifeTime _lifeTime = new();

        private void Awake()
        {
            textUGUI ??= GetComponent<TMP_Text>();
        }
        
#if ODIN_INSPECTOR
        [Button]
#endif
        public void Apply()
        {
            var value = localization.GetLocalizedString();
            
            textUGUI ??= GetComponent<TMP_Text>();
            textUGUI.SetValue(value);
        }

        private void OnEnable()
        {
            _lifeTime.Restart();
            
            localization.AsObservable()
                .Do(x => textUGUI.SetValue(x))
                .Subscribe()
                .AddTo(_lifeTime);
        }

        private void OnDisable()
        {
            _lifeTime.Restart();
        }
    }
}