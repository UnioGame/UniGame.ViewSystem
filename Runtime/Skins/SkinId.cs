using System;

[Serializable]
#if ODIN_INSPECTOR
[Sirenix.OdinInspector.InlineProperty]
[Sirenix.OdinInspector.ValueDropdown("@UniGame.ViewSystem.Editor.EditorAssets.ViewSystemEditorAsset.GetSkins()")]
#endif
public struct SkinId
{

    #region static data

    public static implicit operator string(SkinId v)
    {
        return v.id;
    }

    public static implicit operator SkinId(string v)
    {
        return new SkinId() { id = v };
    }

    #endregion    

    public string id;

    public override string ToString() => id;

    public override int GetHashCode() => string.IsNullOrEmpty(id) ? 0 :  id.GetHashCode();

    public SkinId FromString(string value)
    {
        id = value;
        return this;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is SkinId skinId))
            return false;
        return !string.IsNullOrEmpty(id) && id.Equals(skinId.id);
    }
}
