using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageSelectionPage : Page
    {
        LobbySceneGameMode lobbySceneGameMode;

        Image m_portrait;
        TMP_Text m_battleNameText;

        Button m_enterBattleButton;
        Button m_backButton;

        CompositeDisposable m_subscriptions_page = new();

        protected void Awake()
        {
            m_portrait = transform.FindRecursiveByName<Image>("Portrait");
            m_battleNameText = transform.FindRecursiveByName<TMP_Text>("Battle Name Text");

            m_enterBattleButton = transform.FindRecursiveByName<Button>("Enter Button");
            m_backButton = transform.FindRecursiveByName<Button>("Back Button");
        }

        public override void Open()
        {
            m_backButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickBackButton())
                .AddTo(m_subscriptions_page);

            Close();
        }

        void OnClickBackButton()
        {
            // lobbySceneGameMode.NavigateBack();
        }
    }
}
