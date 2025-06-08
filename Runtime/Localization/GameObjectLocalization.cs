namespace UniGame.Localization.Runtime.Components
{
    using System;
    using Cysharp.Threading.Tasks;
    using global::UniGame.Runtime.DataFlow;
    using R3;
    using Runtime;
     
    using UnityEngine;
    using UnityEngine.Localization;

    public class GameObjectLocalization : MonoBehaviour
    {
        public Transform parent;
        public LocalizedAsset<GameObject> localization;
     
        private LifeTime _lifeTime = new();
        private GameObject _instance;
        private GameObject _asset;
        
        private void Awake()
        {
            parent ??= transform;
        }

        private void OnDestroy()
        {
            if(_instance != null)
                DestroyImmediate(_instance);
            _instance = null;
        }

        private void OnEnable()
        {
            _lifeTime.Restart();
            
            localization.AsObservable()
                .Do(UpdateLocalization)
                .Subscribe()
                .AddTo(_lifeTime);
        }

        private void UpdateLocalization(GameObject asset)
        {
            if (asset == _asset) return;
            
            if(_instance!=null) Destroy(_instance);
            
            if (asset == null) return;
            
            _asset = asset;
            _instance = Instantiate(_asset, parent);
            _instance.DespawnWith(_lifeTime);
        }

        private void OnDisable()
        {
            _lifeTime.Restart();
        }
    }
}