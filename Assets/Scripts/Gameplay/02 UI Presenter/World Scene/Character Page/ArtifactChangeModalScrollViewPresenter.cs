using Mathlife.ProjectL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using UniRx;

namespace Mathlife.ProjectL.Gameplay
{
    internal enum EArtifactScrollViewUsage
    {
        ArtifactChangeModal = 0,
        Inventory = 1,
    }

    internal class ArtifactChangeModalScrollViewPresenter : Presenter
    {
        Transform m_content;

        [Inject] InventoryRepository m_inventoryRepository;

        void Awake()
        {
            m_content = transform.FindRecursiveByName<Transform>("Content");
        }

        public void Initialize()
        {
            // TODO: 아티팩트 리스트 변경 구독
            // m_inventoryRepository.Subscribe...

            m_worldSceneManager.GetPage<CharacterPage>()
                .artifactChangeModal
                .SubscribeSlotTypeChangeEvent(slotType =>
                {
                    UpdateView();
                });

            m_worldSceneManager.GetPage<CharacterPage>()
                .artifactChangeModal
                .SubscribeSelectedArtifactChangeEvent(artifact => UpdateView());

            UpdateView();
        }

        public new void UpdateView()
        {
            foreach (Transform slotTrans in m_content)
            {
                if (slotTrans.GetComponent<ArtifactChangeModalSlotPresenter>() == null)
                    continue;

                Destroy(slotTrans.gameObject);
            }

            EEquipmentType equipType = m_worldSceneManager.GetPage<CharacterPage>().artifactChangeModal.slotType;
            foreach (EquipmentModel equipment in m_inventoryRepository.GetSortedEquipmentList(equipType))
            {
                // 필터가 있다면 타입이 일치하는 아티팩트만 렌더링
                ArtifactChangeModalSlotPresenter slot = InstantiateWithInjection<ArtifactChangeModalSlotPresenter>(EPrefabId.ArtifactChangeModalSlot, m_content);
                slot.Initialize(equipment);
            }
        }
    }
}
