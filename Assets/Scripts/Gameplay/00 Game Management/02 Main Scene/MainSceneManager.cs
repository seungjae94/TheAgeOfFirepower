using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mathlife.ProjectL.Gameplay
{
    // �ʱ�ȭ, ��, ����Ʈ ���μ��� ����, UI ������ ó��
    public class MainSceneManager : SerializedMonoBehaviour
    {
        [SerializeField] LoadingScreen m_loadingScreen;

        PageNavigator m_pageNavigator = new();

        async void Start()
        {
            // 1. �ε� ��ũ�� ����
            m_loadingScreen.Show();

            // 2. UI �ʱ�ȭ
            foreach (var presenter in FindObjectsByType<PresenterBase>(FindObjectsSortMode.None))
            {
                LifetimeScope.Find<MainSceneLifetimeScope>().Container.Inject(presenter);
            }
            m_loadingScreen.SetProgress(0.25f);

            Page[] pages = FindObjectsByType<Page>(FindObjectsSortMode.None);
            m_pageNavigator.AddPages(pages);
            m_loadingScreen.SetProgress(0.5f);

            // 3. ī�޶� ���� �� �ε� ��ũ�� �����
            await UniTask.Delay(100);
            m_loadingScreen.Hide();
        }
        public void NavigateHome()
        {
            m_pageNavigator.Home();
        }

        public void NavigateBack()
        {
            m_pageNavigator.Back();
        }

        public void Navigate(EPageId pageId)
        {
            m_pageNavigator.Navigate(pageId);
        }
    }
}
