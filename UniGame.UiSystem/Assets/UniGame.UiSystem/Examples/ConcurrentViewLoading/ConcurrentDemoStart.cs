using System.Collections;
using System.Collections.Generic;
using UiSystem.Assets.UniGame.UiSystem.Examples.ConcurrentViewLoading;
using UniGreenModules.UniGame.UiSystem.Runtime;
using UnityEngine;

public class ConcurrentDemoStart : MonoBehaviour
{
    public GameViewSystemComponent viewSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        //async load views as windows
        viewSystem.CreateWindow<DemoViewConcurrent1>(new ViewModelBase());
        viewSystem.CreateWindow<DemoViewConcurrent2>(new ViewModelBase());
        viewSystem.CreateWindow<DemoViewConcurrent3>(new ViewModelBase());
        viewSystem.CreateWindow<DemoViewConcurrent4>(new ViewModelBase());
        viewSystem.CreateWindow<DemoViewConcurrent5>(new ViewModelBase());
        viewSystem.CreateWindow<DemoViewConcurrent6>(new ViewModelBase());
        viewSystem.CreateWindow<DemoViewConcurrent7>(new ViewModelBase());
        viewSystem.CreateWindow<DemoViewConcurrent8>(new ViewModelBase());
        viewSystem.CreateWindow<DemoViewConcurrent9>(new ViewModelBase());
        
        //async load views as screens
        viewSystem.CreateScreen<DemoViewConcurrent1>(new ViewModelBase());
        viewSystem.CreateScreen<DemoViewConcurrent2>(new ViewModelBase());
        viewSystem.CreateScreen<DemoViewConcurrent3>(new ViewModelBase());
        viewSystem.CreateScreen<DemoViewConcurrent4>(new ViewModelBase());
        viewSystem.CreateScreen<DemoViewConcurrent5>(new ViewModelBase());
        viewSystem.CreateScreen<DemoViewConcurrent6>(new ViewModelBase());
        viewSystem.CreateScreen<DemoViewConcurrent7>(new ViewModelBase());
        viewSystem.CreateScreen<DemoViewConcurrent8>(new ViewModelBase());
        viewSystem.CreateScreen<DemoViewConcurrent9>(new ViewModelBase());
    }

}
