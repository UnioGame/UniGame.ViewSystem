using UiSystem.Assets.UniGame.UiSystem.Examples.ConcurrentViewLoading;
using UniGame.UiSystem.Runtime;
using UnityEngine;

namespace UniGame.UiSystem.Examples.ConcurrentViewLoading
{
    using UniModules.UniGame.UISystem.Runtime.Abstract;

    public class ConcurrentDemoStart : MonoBehaviour
    {
        public GameViewSystemAsset viewSystem;
    
        // Start is called before the first frame update
        void Start()
        {
            //async load views as windows
            viewSystem.OpenWindow<DemoViewConcurrent1>(new ViewModelBase());
            viewSystem.OpenWindow<DemoViewConcurrent2>(new ViewModelBase());
            viewSystem.OpenWindow<DemoViewConcurrent3>(new ViewModelBase());
            viewSystem.OpenWindow<DemoViewConcurrent4>(new ViewModelBase());
            viewSystem.OpenWindow<DemoViewConcurrent5>(new ViewModelBase());
            viewSystem.OpenWindow<DemoViewConcurrent6>(new ViewModelBase());
            viewSystem.OpenWindow<DemoViewConcurrent7>(new ViewModelBase());
            viewSystem.OpenWindow<DemoViewConcurrent8>(new ViewModelBase());
            viewSystem.OpenWindow<DemoViewConcurrent9>(new ViewModelBase());
        
            //async load views as screens
            viewSystem.OpenScreen<DemoViewConcurrent1>(new ViewModelBase());
            viewSystem.OpenScreen<DemoViewConcurrent2>(new ViewModelBase());
            viewSystem.OpenScreen<DemoViewConcurrent3>(new ViewModelBase());
            viewSystem.OpenScreen<DemoViewConcurrent4>(new ViewModelBase());
            viewSystem.OpenScreen<DemoViewConcurrent5>(new ViewModelBase());
            viewSystem.OpenScreen<DemoViewConcurrent6>(new ViewModelBase());
            viewSystem.OpenScreen<DemoViewConcurrent7>(new ViewModelBase());
            viewSystem.OpenScreen<DemoViewConcurrent8>(new ViewModelBase());
            viewSystem.OpenScreen<DemoViewConcurrent9>(new ViewModelBase());
        }

    }
}
