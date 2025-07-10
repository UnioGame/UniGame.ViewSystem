namespace UniGame.ViewSystem.Runtime.Binding
{
    using System;
    using AddressableTools.Runtime;
    using R3;
    using UniGame.Runtime.Rx.Runtime.Extensions;
    using TMPro;
    using UniGame.Runtime.Common;
     
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.UI;

    public class ViewFieldBinder
    {
        public IView Bind(IView view, ref BindDataConnection bindData)
        {
            var sourceValue = bindData.sourceValue;
            var targetValue = bindData.targetValue;
            
            if (sourceValue == null || targetValue == null) return view;

            switch (sourceValue)
            {
                case Observable<Sprite> observable:
                    return Bind(view, observable, ref bindData);
                case Observable<Unit> observable:
                    return Bind<Unit>(view, observable, ref bindData);
                case Observable<string> observable:
                    return Bind(view, observable, ref bindData);
                case Observable<bool> observable:
                    return Bind(view, observable, ref bindData);
                case Observable<int> observable:
                    return Bind(view, observable, ref bindData);
                case Observable<Color> observable:
                    return Bind(view, observable, ref bindData);
                case Observable<float> observable:
                    return Bind(view, observable, ref bindData);
                case Observable<Texture> observable:
                    return Bind(view, observable, ref bindData);
                case Observable<AssetReferenceT<Sprite>> observable:
                    return Bind(view, observable, ref bindData);
                case Observable<AddressableValue<Sprite>> observable:   
                    return Bind(view, observable, ref bindData);
                case SignalValueProperty<bool> observable:   
                    return Bind(view, observable, ref bindData);
            }

            return view;
        }

        public IView Bind(IView view, Observable<int> observable, ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;

            return targetValue switch
            {
                TextMeshProUGUI value => view.Bind(observable, value),
                TextMeshPro value => view.Bind(observable, value),
                TMP_InputField value => view.Bind(observable, value),
                _ => Bind<int>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view, Observable<Color> observable, ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;

            return targetValue switch
            {
                Image value => view.Bind(observable, value),
                TextMeshProUGUI value => view.Bind(observable, value),
                TextMeshPro value => view.Bind(observable, value),
                TMP_InputField value => view.Bind(observable, value),
                Button value => view.Bind(observable, value),
                _ => Bind<Color>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view, Observable<float> observable, ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;

            return targetValue switch
            {
                TextMeshProUGUI value => view.Bind(observable, value),
                TextMeshPro value => view.Bind(observable, value),
                TMP_InputField value => view.Bind(observable, value),
                Slider value => view.Bind(observable, value),
                _ => Bind<float>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view, Observable<bool> observable, ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;

            return targetValue switch
            {
                Image value => view.Bind(observable, value),
                GameObject value => view.Bind(observable, value),
                RawImage value => view.Bind(observable, value),
                Toggle value => view.Bind(observable, value),
                CanvasGroup value => view.Bind(observable, value),
                _ => Bind<bool>(view, observable, ref bindData)
            };
        }

        public IView Bind<T>(IView view, Observable<T> observable, ref BindDataConnection bindData)
        {
            if (observable == null) return view;
            
            var targetValue = bindData.targetValue;
            
            return targetValue switch
            {
                ReactiveProperty<T> value => view.Bind(observable, value),
                ReactiveCommand<T> value => view.Bind(observable, value),
                Observer<T> value => view.Bind(observable, value),
                ISubject<T> value => view.Bind(observable, value),
                IObserver<T> value => view.Bind(observable, value),
                _ => view
            };
        }
        
        public IView Bind(IView view,Observable<string> observable,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                TextMeshProUGUI value => view.Bind(observable, value),
                TextMeshPro value => view.Bind(observable, value),
                TMP_InputField value => view.Bind(observable, value),
                _ => Bind<string>(view, observable, ref bindData)
            };
        }

        public IView Bind(IView view,Observable<Sprite> observable,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                Image image => view.Bind(observable, image),
                RawImage rawImage => view.Bind(observable, rawImage),
                _ => Bind<Sprite>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view,Observable<AssetReferenceT<Sprite>> observable,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                Image image => view.Bind(observable, image),
                _ => Bind<AssetReferenceT<Sprite>>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view,Observable<AddressableValue<Sprite>> observable,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                Image image => view.Bind(observable, image),
                _ => Bind<AddressableValue<Sprite>>(view, observable, ref bindData)
            };
        }
        
        public IView Bind(IView view,SignalValueProperty<bool> source,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                Button value => view.Bind(value, source),
                Toggle value => view.Bind(value, source),
                Observable<Unit> value=> view.Bind(value,source),
                Observable<bool> value=> view.Bind(value,source),
                _ => view
            };  
        }
        
        public IView Bind(IView view,Observable<Texture> observable,ref BindDataConnection bindData)
        {
            var targetValue = bindData.targetValue;
            return targetValue switch
            {
                RawImage rawImage => view.Bind(observable, rawImage),
                _ => Bind<Texture>(view, observable, ref bindData)
            };
        }
    }
}