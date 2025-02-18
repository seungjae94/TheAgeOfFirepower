using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Graphs;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    internal class PartyMemberChangeModal : Presenter
    {


        //public async UniTask Show(EEquipmentType slotType)
        //{
        //    m_slotType = slotType;

        //    EquipmentModel currentEquipment = m_worldSceneManager.GetPage<PartyPage>().selectedCharacter.GetEquipment(slotType);
        //    m_selectedEquipment = currentEquipment;

        //    UpdateCurrentEquipmentView(currentEquipment);
        //    UpdateSelectedEquipmentView();
        //    UpdateGridView();

        //    await m_canvasGroup.Show(k_fadeTime);
        //}

        //public async UniTask Hide()
        //{
        //    await m_canvasGroup.Hide(k_fadeTime);
        //}
        protected override void InitializeView()
        {
            throw new NotImplementedException();
        }

        protected override void SubscribeDataChange()
        {
            throw new NotImplementedException();
        }

        protected override void SubscribeUserInteractions()
        {
            throw new NotImplementedException();
        }
    }
}
