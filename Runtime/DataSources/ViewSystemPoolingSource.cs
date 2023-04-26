namespace UniGame.ViewSystem.Runtime.DataSources
{
    using System.Collections.Generic;
    using Context.Runtime;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.ViewSystem.Runtime.Extensions;
    using UniModules.UniGame.ViewSystem.Runtime.Settings;
    using UnityEngine;
    
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/View System Pooling", fileName = "View Pooling Asset")]
    public class ViewSystemPoolingSource : ScriptableObject,IAsyncDataSource
    {
        public List<AssetReferenceViewSettings> sources = new List<AssetReferenceViewSettings>();

        public UniTask<IContext> RegisterAsync(IContext context)
        {
            foreach (var reference in sources)
                reference.Warmup(context.LifeTime).Forget();

            return UniTask.FromResult(context);
        }
    }
}
