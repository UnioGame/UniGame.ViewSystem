using System;
using UniRx;
using UnityEngine;

public class ViewSkinTagComponent : MonoBehaviour, IViewSkinTag
{

    public SkinId skinTag;

    public string SkinTag => skinTag;


#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button]
#endif
    public void ApplySkin(string skinTagName)
    {
        var newSkin = new SkinId() {id = skinTagName};
        skinTag = newSkin;
        
        //rebuild view system update skin id's list
        MessageBroker.Default.Publish(new SettingsRebuildMessage());
    }
    
}