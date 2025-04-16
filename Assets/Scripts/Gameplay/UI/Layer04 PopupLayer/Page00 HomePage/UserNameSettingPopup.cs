using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class UserNameSettingPopup : PopupPresenter
    {
        // Alias
        private static GameProgressState GameProgressState => GameState.Inst.gameProgressState;

        // Constant
        private const float k_openDuration = 0.25f;

        // View
        [SerializeField]
        private RectTransform windowTransform;

        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private Button okButton;

        // Field
        private Tween openTween;
        private Tween closeTween;
        private readonly CompositeDisposable disposables = new();

        // State
        public override void Initialize()
        {
            base.Initialize();

            openTween = windowTransform.DOScale(new Vector3(1f, 1f, 1f), k_openDuration)
                .From(new Vector3(0f, 0f, 1f))
                .SetAutoKill(false)
                .Pause();

            closeTween = windowTransform.DOScale(new Vector3(0f, 0f, 1f), k_openDuration)
                .From(new Vector3(1f, 1f, 1f))
                .SetAutoKill(false)
                .Pause();
        }

        public override async UniTask OpenWithAnimation()
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.PopupOpen);
            
            // 블러 적용
            BlurPopup blurPopup = Find<BlurPopup>();
            blurPopup.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            await blurPopup.OpenWithAnimation();

            await base.OpenWithAnimation();

            okButton.OnClickAsObservable()
                .Subscribe(OnClickOKButton)
                .AddTo(disposables);

            okButton.interactable = false;

            inputField.onEndEdit
                .AsObservable()
                .Subscribe(OnInputFieldEndEdit)
                .AddTo(disposables);

            openTween.Restart();
            await openTween.AwaitForComplete();
        }

        public override async UniTask CloseWithAnimation()
        {
            disposables.Clear();

            closeTween.Restart();
            await closeTween.AwaitForComplete();

            // 블러 제거
            await Find<BlurPopup>().CloseWithAnimation();

            await base.CloseWithAnimation();
        }

        private void OnDestroy()
        {
            openTween.Kill();
            closeTween.Kill();
            disposables.Dispose();
        }

        // 구독
        private void OnInputFieldEndEdit(string userName)
        {
            bool isValid = GameProgressState.TryMakeValidUserName(userName, out string newUserName);
            inputField.SetTextWithoutNotify(newUserName);
            okButton.interactable = isValid;
        }

        private void OnClickOKButton(Unit _)
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.PopupClose);
            inputField.interactable = false;
            string userName = inputField.text;

            if (GameProgressState.IsUserNameValid(userName))
            {
                GameProgressState.userNameRx.Value = userName;
                GameState.Inst.Save();
                CloseWithAnimation().Forget();
            }
        }
    }
}