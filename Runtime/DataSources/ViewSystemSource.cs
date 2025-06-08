namespace UniGame.ViewSystem.Runtime.DataSources
{
    using UniGame.UiSystem.Runtime;
    using UniGame.Core.Runtime;
    using UniGame.AddressableTools.Runtime;
    using Runtime;
    using Context.Runtime;
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Serialization;

    [Serializable]
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/" + nameof(ViewSystemSource), fileName = nameof(ViewSystemSource))]
    public class ViewSystemSource : DataSourceAsset<IGameViewSystem>
    {
        [FormerlySerializedAs("View System Source")]
        [SerializeField]
        private AssetReferenceGameObject viewSystemSource;
        
        protected override async UniTask<IGameViewSystem> CreateInternalAsync(IContext context)
        {
            var startDate = DateTime.Now;
            var lifeTime = context.LifeTime;
            
            var viewSystemAsset = await viewSystemSource
                .LoadGameObjectAssetTaskAsync(lifeTime);
            var viewObject = Instantiate(viewSystemAsset.gameObject);
            var viewAsset = viewObject.GetComponent<GameViewSystemAsset>();

            DontDestroyOnLoad(viewObject);
            viewObject.DestroyWith(lifeTime);

            var time = DateTime.Now - startDate;
            Debug.Log($"{nameof(IGameViewSystem)} {nameof(ViewSystemSource)} Duration Before {time.TotalMilliseconds} Start {DateTime.Now.ToLongTimeString()}");

            await UniTask.WaitUntil(() => viewAsset.IsReady == true);

            time = DateTime.Now - startDate;
            Debug.Log($"{nameof(IGameViewSystem)} {nameof(ViewSystemSource)} Duration Ready {time.TotalMilliseconds} Start {DateTime.Now.ToLongTimeString()}");

            return viewAsset;
        }
    }
}