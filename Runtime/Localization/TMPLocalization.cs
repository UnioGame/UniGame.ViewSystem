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
        public LocalizedString localization;

        private TextMeshPro _text;
        private TextMeshProUGUI _textUGUI;
        private LifeTime _lifeTime = new();

        private void Awake()
        {
            _text = GetComponent<TextMeshPro>();
            _textUGUI = GetComponent<TextMeshProUGUI>();
        }
        
        [Button]
        public void Apply()
        {
            var value = localization.GetLocalizedString();
            
            _text = GetComponent<TextMeshPro>();
            _textUGUI = GetComponent<TextMeshProUGUI>();
            _text.SetValue(value);
            _textUGUI.SetValue(value);
        }

        private void OnEnable()
        {
            _lifeTime.Restart();
            
            localization.AsObservable()
                .Do(x => _textUGUI.SetValue(x))
                .Do(x => _text.SetValue(x))
                .Subscribe()
                .AddTo(_lifeTime);
        }

        private void OnDisable()
        {
            _lifeTime.Restart();
        }
    }
}