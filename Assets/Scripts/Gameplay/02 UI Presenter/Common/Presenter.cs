using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public abstract class Presenter : MonoBehaviour
    {
        [Inject] protected IObjectResolver m_container;
        [Inject] protected WorldSceneManager m_worldSceneManager;
        [Inject] protected GameDataDB m_gameDataDB;

        protected T InstantiateWithInjection<T>(EPrefabId prefabId, Transform trans) where T : Component
        {
            T inst = m_gameDataDB.Instantiate<T>(prefabId, trans);

            if (inst == null)
            {
                Debug.LogError($"Failed to instantiate. Check if given prefab has {typeof(T).Name} component.");
                return null;
            }

            m_container.Inject(inst);
            return inst;
        }

        // 뷰를 초기화 할 때 사용. 생성 후 1번만 호출.
        // ex) 카드 생성 후 캐릭터 데이터 렌더링
        protected virtual void InitializeView() { }

        // 뷰를 업데이트 할 때 사용. 생성 후 반복해서 호출 가능.
        // ex) 스크롤 뷰 생성 후 하위 카드 재생성
        protected virtual void UpdateView() { }
    }
}
