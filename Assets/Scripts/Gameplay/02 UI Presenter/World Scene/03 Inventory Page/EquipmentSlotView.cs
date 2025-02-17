using Mathlife.ProjectL.Utils;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class EquipmentSlotView : MonoBehaviour
    {
        [SerializeField] Button m_button;
        [SerializeField] Image m_iconImage;
        [SerializeField] CanvasGroup m_equippedMark;
        [SerializeField] CanvasGroup m_selectedMark;

        IDisposable m_onClickSub;

        public void Render(EquipmentModel equipment, bool selected, Action<EquipmentModel> onClickAction)
        {
            if (m_onClickSub != null)
                m_onClickSub.Dispose();

            m_onClickSub = m_button.OnClickAsObservable()
                .Subscribe(_ => onClickAction(equipment))
                .AddTo(gameObject);

            m_iconImage.sprite = equipment.icon;

            if (equipment.owner != null)
                m_equippedMark.Show();
            else
                m_equippedMark.Hide();

            if (selected)
                m_selectedMark.Show();
            else
                m_selectedMark.Hide();
        }
    }
}
