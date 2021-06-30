using System;

[Serializable]
#if ODIN_INSPECTOR
[Sirenix.OdinInspector.InlineProperty]
[Sirenix.OdinInspector.ValueDropdown("@UniGame.ViewSystem.Editor.EditorAssets.ViewSystemEditorAsset.GetSkins()")]
#endif
public struct SkinId
{
    public string id;
    
    
    public static implicit operator string(SkinId v)
    {
        return v.id;
    }

    public static implicit operator SkinId(string v)
    {
        return new SkinId() { id = v };
    }
    
    public override string ToString() => id;

    public override int GetHashCode() => id.GetHashCode();

    public SkinId FromString(string value)
    {
        id = value;
        return this;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is SkinId skinId))
            return false;
        return id.Equals(skinId.id);
    }
}
