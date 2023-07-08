namespace UniGame.Tests.UnitTests.Editor.ViewSystem
{
    using System.Collections.Generic;
    using UniGame.UiSystem.Runtime.Settings;
    using NUnit.Framework;
    using UniGame.UiSystem.Runtime;
    using UniGame.Core.Runtime.SerializableType;
    using UniGame.ViewSystem.Runtime;
    using UnityEngine.AddressableAssets;

    public class ViewSelectionTests
    {
        private class SkinViewModel : ViewModelBase { }
        private class DefaultViewModel : ViewModelBase { }

        private ViewModelTypeMap _viewModelTypeMap = new ViewModelTypeMap();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var viewsSkinFirst = new UiReferenceList
            {
                references = new List<UiViewReference>
                {
                    new UiViewReference
                    {
                        AssetGUID     = "",
                        View          = new AssetReferenceGameObject(""),
                        ViewName      = "Skin",
                        Tag           = "Skin",
                        Type          = typeof(View<SkinViewModel>),
                        ModelType     = typeof(SkinViewModel),
                        ViewModelType = typeof(SkinViewModel)
                    },
                    new UiViewReference
                    {
                        AssetGUID     = "",
                        View          = new AssetReferenceGameObject(""),
                        ViewName      = "Default",
                        Tag           = "",
                        Type          = typeof(View<SkinViewModel>),
                        ModelType     = typeof(SkinViewModel),
                        ViewModelType = typeof(SkinViewModel)
                    }
                }
            };

            foreach (var uiReference in viewsSkinFirst.references)
            {
                _viewModelTypeMap.viewsTypeMap.Add(uiReference);
            }
            
            var viewsDefaultFirst = new UiReferenceList
            {
                references = new List<UiViewReference>
                {
                    new UiViewReference
                    {
                        AssetGUID     = "",
                        View          = new AssetReferenceGameObject(""),
                        ViewName      = "Default",
                        Tag           = "",
                        Type          = typeof(View<DefaultViewModel>),
                        ModelType     = typeof(DefaultViewModel),
                        ViewModelType = typeof(DefaultViewModel)
                    },
                    new UiViewReference
                    {
                        AssetGUID     = "",
                        View          = new AssetReferenceGameObject(""),
                        ViewName      = "Skin",
                        Tag           = "Skin",
                        Type          = typeof(View<DefaultViewModel>),
                        ModelType     = typeof(DefaultViewModel),
                        ViewModelType = typeof(DefaultViewModel)
                    }
                }
            };
            
            foreach (var uiReference in viewsDefaultFirst.references)
            {
                _viewModelTypeMap.viewsTypeMap.Add(uiReference);
            }
        }

        [Test]
        public void GetDefaultViewForEmptySkinTag()
        {
            var view = _viewModelTypeMap.FindView(typeof(View<SkinViewModel>).Name);
            Assert.That(view.ViewName == "Default");
        }

        [Test]
        public void GetSkin()
        {
            var view = _viewModelTypeMap.FindView(typeof(View<DefaultViewModel>).Name, "Skin");
            Assert.That(view.ViewName == "Skin");
        }
    }
}