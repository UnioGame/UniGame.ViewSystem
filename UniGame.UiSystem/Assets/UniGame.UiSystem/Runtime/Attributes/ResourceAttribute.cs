using System;

namespace UniGreenModules.UniGame.UiSystem.Runtime.Attributes
{
    public class ResourceAttribute : Attribute
    {
        public string Address;
        public ResourceAttribute(string address)
        {
            this.Address = address;
        }
    }
}