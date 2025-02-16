using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    internal class EquipmentSlotGridView : MonoBehaviour
    {
        [SerializeField] Transform m_contents;

        List<EquipmentSlotView> m_equipmentSlots = new();

        public void Render(
            List<EquipmentModel> equipments, 
            Func<Transform, EquipmentModel, EquipmentSlotView> equipmentSlotViewFactoryMethod)
        {
            foreach (EquipmentSlotView slot in m_equipmentSlots)
            {
                Destroy(slot.gameObject);
            }
            m_equipmentSlots.Clear();

            foreach (EquipmentModel equipment in equipments)
            {
                EquipmentSlotView slot = equipmentSlotViewFactoryMethod(m_contents, equipment);
                m_equipmentSlots.Add(slot);
            }
        }
    }
}
