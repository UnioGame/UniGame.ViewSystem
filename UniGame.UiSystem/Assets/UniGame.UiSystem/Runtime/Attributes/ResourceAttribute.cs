using System;

namespace UniGreenModules.UniGame.UiSystem.Runtime.Attributes
{
    // атрибут не используется и никакой логики для перегоне его в So не найдено,
    // хотя она вроде планировкалась
    public class ResourceAttribute : Attribute
    {
        public string Address;
        public ResourceAttribute(string address)
        {
            this.Address = address;
        }
    }
}