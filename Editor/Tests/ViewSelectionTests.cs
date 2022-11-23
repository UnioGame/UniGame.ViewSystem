namespace UniGame.Tests.UnitTests.Editor.ViewSystem
{
    using System.Collections.Generic;
    using UniGame.UiSystem.Runtime.Settings;
    using NUnit.Framework;
    using UniGame.UiSystem.Runtime;
    using UniGame.Core.Runtime.SerializableType;
    using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;
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
                        Type          = typeof(UiView<SkinViewModel>),
                        ModelType     = typeof(SkinViewModel),
                        ViewModelType = typeof(SkinViewModel)
                    },
                    new UiViewReference
                    {
                        AssetGUID     = "",
                        View          = new AssetReferenceGameObject(""),
                        ViewName      = "Default",
                        Tag           = "",
                        Type          = typeof(UiView<SkinViewModel>),
                        ModelType     = typeof(SkinViewModel),
                        ViewModelType = typeof(SkinViewModel)
                    }
                }
            };
            _viewModelTypeMap.viewsTypeMap.Add(new SType {Type = typeof(UiView<SkinViewModel>) }, viewsSkinFirst);

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
                        Type          = typeof(UiView<DefaultViewModel>),
                        ModelType     = typeof(DefaultViewModel),
                        ViewModelType = typeof(DefaultViewModel)
                    },
                    new UiViewReference
                    {
                        AssetGUID     = "",
                        View          = new AssetReferenceGameObject(""),
                        ViewName      = "Skin",
                        Tag           = "Skin",
                        Type          = typeof(UiView<DefaultViewModel>),
                        ModelType     = typeof(DefaultViewModel),
                        ViewModelType = typeof(DefaultViewModel)
                    }
                }
            };
            _viewModelTypeMap.viewsTypeMap.Add(new SType { Type = typeof(UiView<DefaultViewModel>) }, viewsDefaultFirst);
        }

        [Test]
        public void GetDefaultViewForEmptySkinTag()
        {
            var view = _viewModelTypeMap.FindView(typeof(UiView<SkinViewModel>));
            Assert.That(view.ViewName == "Default");
        }

        [Test]
        public void GetSkin()
        {
            var view = _viewModelTypeMap.FindView(typeof(UiView<DefaultViewModel>), "Skin");
            Assert.That(view.ViewName == "Skin");
        }
    }
}