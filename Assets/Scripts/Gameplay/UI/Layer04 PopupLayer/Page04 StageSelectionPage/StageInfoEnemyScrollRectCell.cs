using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageInfoEnemyScrollRectCell
    : SimpleScrollRectCell<Enemy,  SimpleScrollRectContext>
    {
        [SerializeField]
        private Image enemyImage;

        [SerializeField]
        private TextMeshProUGUI enemyLevelText;
        
        public override void UpdateContent(Enemy itemData)
        {
            enemyImage.sprite = itemData.artyGameData.enemySprite;
            enemyLevelText.text = $"Lv. {itemData.level}";
        }
    }
}