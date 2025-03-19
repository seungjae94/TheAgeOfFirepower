using System;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.UI;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class ArtyPageSelectedArtyView : AbstractView
    {
        // Alias
        private ArtyPage ArtyPage => Presenter.Find<ArtyPage>();
        
        // View
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelText;
        
        [SerializeField] private Slider expSlider;
        [SerializeField] private TextMeshProUGUI currentLevelExpText;
        [SerializeField] private TextMeshProUGUI needExpText;
        
        [SerializeField] private TextMeshProUGUI maxHpText;
        [SerializeField] private TextMeshProUGUI mobilityText;
        [SerializeField] private TextMeshProUGUI atkText;
        [SerializeField] private TextMeshProUGUI defText;
        
        // Field
        private readonly CompositeDisposable disposables = new();

        // Lifetime

        public override void Draw()
        {
            base.Draw();

            ArtyPage.selectedArtyIndexRx
                .DistinctUntilChanged()
                .Subscribe(index => UpdateView())
                .AddTo(disposables);
            
            UpdateView();
        }

        public override void Clear()
        {
            base.Clear();

            disposables.Clear();
        }

        void OnDestroy()
        {
            disposables.Dispose();
        }

        void UpdateView()
        {
            ArtyModel arty = ArtyPage.SelectedArty;
            
            if (arty == null)
            {
                Debug.LogError("[ArtyPageSelectedArtyView] 선택된 화포가 없습니다.");
                throw new ArtyPageNoArtySelectedException("[ArtyPageSelectedArtyView] 선택된 화포가 없습니다.");
            }

            portrait.sprite = arty.Sprite;
            nameText.text = arty.displayName;
            levelText.text = arty.levelRx.Value.ToString();
            
            expSlider.value = (float)arty.currentLevelExp / arty.needExp;
            currentLevelExpText.text = arty.currentLevelExp.ToString();
            needExpText.text = arty.needExp.ToString();

            maxHpText.text = arty.GetMaxHp().ToString();
            mobilityText.text = arty.GetMobility().ToString();
            atkText.text = arty.GetAtk().ToString();
            defText.text = arty.GetDef().ToString();
        }
    }
}
