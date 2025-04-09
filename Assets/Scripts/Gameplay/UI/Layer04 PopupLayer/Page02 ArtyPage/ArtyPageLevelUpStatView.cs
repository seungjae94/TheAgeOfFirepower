using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ArtyPageLevelUpStatView : AbstractView
    {
        // Alias
        private static ArtyModel Arty => Presenter.Find<ArtyPage>().SelectedArty;
        private static ArtyPageLevelUpPopup popup => Presenter.Find<ArtyPageLevelUpPopup>();
        
        // Component (고정값)
        [SerializeField]
        private TextMeshProUGUI beforeLevelText;
        
        [SerializeField]
        private TextMeshProUGUI maxHpText;
        
        [SerializeField]
        private TextMeshProUGUI mobText;
        
        [SerializeField]
        private TextMeshProUGUI atkText;
        
        [SerializeField]
        private TextMeshProUGUI defText;
        
        // Component (변동값)
        [SerializeField]
        private TextMeshProUGUI afterLevelText;
        
        [SerializeField]
        private TextMeshProUGUI maxHpChangeText;
        
        [SerializeField]
        private TextMeshProUGUI mobChangeText;
        
        [SerializeField]
        private TextMeshProUGUI atkChangeText;
        
        [SerializeField]
        private TextMeshProUGUI defChangeText;
        
        [SerializeField]
        private Slider expSlider;

        [SerializeField]
        private TextMeshProUGUI expGainText;
        
        [SerializeField]
        private TextMeshProUGUI needExpText;
        
        // Field
        private readonly CompositeDisposable disposables = new();
        
        public override void Draw()
        {
            base.Draw();
            
            popup.expGainRx
                .Subscribe(OnExpGainChange)
                .AddTo(disposables);
            
            // 고정값
            beforeLevelText.text = Arty.levelRx.Value.ToString();
            maxHpText.text = Arty.GetMaxHp().ToString();
            mobText.text = Arty.GetMobility().ToString();
            atkText.text = Arty.GetAtk().ToString();
            defText.text = Arty.GetDef().ToString();
            
            // 변동값
            afterLevelText.text = Arty.levelRx.Value.ToString();
            maxHpChangeText.text = "";
            mobChangeText.text = "";
            atkChangeText.text = "";
            defChangeText.text = "";
            
            expSlider.value = 0f;
            expGainText.text = 0.ToString();
            needExpText.text = Arty.NeedExp.ToString();
        }
        
        public override void Clear()
        {
            base.Clear();

            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void OnExpGainChange(long expGain)
        {
            // after level 계산
            
            // tween
        }
    }
}