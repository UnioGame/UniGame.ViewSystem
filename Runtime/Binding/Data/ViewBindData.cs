namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [Serializable]
    public class ViewBindData : MonoBehaviour
    {
        [ListDrawerSettings(CustomAddFunction = nameof(AddValue),ListElementLabelName = "field")]
        public List<BindValue> values = new List<BindValue>();
        
        public void AddValue()
        {
            var value = new BindValue()
            {
                view = gameObject,
                enabled = true,
            };
            values.Add(value);
        }
    }
}