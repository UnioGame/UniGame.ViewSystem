namespace UniGame.Localization.Runtime.Components
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.Runtime.DataFlow;
    using R3;
    using Sirenix.OdinInspector;
    using TMPro;
    using Runtime;
     
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