namespace Taktika.UI.Common.Views
{
    using Cysharp.Threading.Tasks;
    using Taktika.UI.Common.ViewModels;
    using UnityEngine.UI;
    using TMPro;
    using UniGame.UiSystem.Runtime;
    using UniRx;
    using UnityEngine;

    public class DeviceInfoView : UiView<DeviceInfoViewModel>
    {
        [SerializeField] private TextMeshProUGUI _userId;
        [SerializeField] private TextMeshProUGUI _deviceGUID;
        [SerializeField] private Button          _userIdButton;
        [SerializeField] private Button          _guidButton;

        private string _deviceId;

        protected override async UniTask OnInitialize(DeviceInfoViewModel model)
        {
            await base.OnInitialize(model);

            _deviceId = SystemInfo.deviceUniqueIdentifier;

            _userId.text      = model.UserId;
            _deviceGUID.text  = $"{Application.version}_{_deviceId}";
            
            _userIdButton.onClick.AsObservable().Subscribe(_ => GUIUtility.systemCopyBuffer = _userId.text).AddTo(this);
            _guidButton.onClick.AsObservable().Subscribe(_ => GUIUtility.systemCopyBuffer   = _deviceId).AddTo(this);
        }
    }
}
