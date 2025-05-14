namespace UniGame.Localization.Runtime.Components
{
    using global::UniModules.UniCore.Runtime.DataFlow;
    using Sirenix.OdinInspector;
    using TMPro;
    using UniModules.UniGame.Localization.Runtime;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Localization;
    using ViewSystem.Runtime;

    public class TMPLocalization : MonoBehaviour
    {
        public TextMeshPro text;
        public TextMeshProUGUI textUGUI;
        public LocalizedString localization;
        
        private LifeTime _lifeTime = new();

        private void Awake()
        {
            text ??= GetComponent<TextMeshPro>();
            textUGUI ??= GetComponent<TextMeshProUGUI>();
        }
        
        [Button]
        public void Apply()
        {
            var value = localization.GetLocalizedString();
            
            text ??= GetComponent<TextMeshPro>();
            textUGUI ??= GetComponent<TextMeshProUGUI>();
            text.SetValue(value);
            textUGUI.SetValue(value);
        }

        private void OnEnable()
        {
            _lifeTime.Restart();
            
            localization.AsObservable()
                .Do(x => textUGUI.SetValue(x))
                .Do(x => text.SetValue(x))
                .Subscribe()
                .AddTo(_lifeTime);
        }

        private void OnDisable()
        {
            _lifeTime.Restart();
        }
    }
}