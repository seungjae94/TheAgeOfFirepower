using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ArtyPageLevelUpStatView : AbstractView
    {
        private const float SLIDER_SPEED = 2f;
        
        // Alias
        private static ExpGameData ExpGameData => GameState.Inst.gameDataLoader.GetExpData();
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
        private TextMeshProUGUI expGainText;
        
        [SerializeField]
        private TextMeshProUGUI needExpText;
        
        [SerializeField]
        private Slider expSlider;
        
        // Field
        private readonly CompositeDisposable disposables = new();

        private float currentTargetValue = 0f; // Level[정수] + SliderValue[소수]
        private float prevTargetValue = 0f; // Level[정수] + SliderValue[소수]
        private float nextTargetValue = 0f; // Level[정수] + SliderValue[소수]
        
        public override void Draw()
        {
            base.Draw();
            
            popup.ExpGainRx
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
            
            expGainText.text = Arty.CurrentLevelExp.ToString();
            needExpText.text = Arty.NeedExp.ToString();
            expSlider.value = (float)Arty.CurrentLevelExp / Arty.NeedExp;
            
            // 변수 초기화
            currentTargetValue = Arty.levelRx.Value + expSlider.value;
            prevTargetValue = Arty.levelRx.Value + expSlider.value;
            nextTargetValue = Arty.levelRx.Value + expSlider.value;
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
            int beforeLevel = Arty.levelRx.Value;
            int afterLevel = beforeLevel;
            
            long afterTotalExp = Arty.totalExpRx.Value + expGain;
            
            for (int i = beforeLevel + 1; i <= 100; ++i)
            {
                long totalExpAtLevel = ExpGameData.totalExpAtLevelList[i];

                if (afterTotalExp < totalExpAtLevel)
                {
                    afterLevel = i - 1;
                    break;
                }
            }
            
            long currentLevelExp = afterTotalExp - ExpGameData.totalExpAtLevelList[afterLevel]; 
            long needExp = ExpGameData.needExpAtLevelList[afterLevel];
            int maxHpChange = Arty.GetMaxHp(afterLevel) - Arty.GetMaxHp();
            int mobChange = Arty.GetMobility(afterLevel) - Arty.GetMobility();
            int atkChange = Arty.GetAtk(afterLevel) - Arty.GetAtk();
            int defChange = Arty.GetDef(afterLevel) - Arty.GetDef();
            
            afterLevelText.text = afterLevel.ToString();
            maxHpChangeText.text = maxHpChange > 0 ?  $"+{maxHpChange}" : "";
            mobChangeText.text = mobChange > 0 ?  $"+{mobChange}" : "";
            atkChangeText.text = atkChange > 0 ?  $"+{atkChange}" : "";
            defChangeText.text = defChange > 0 ?  $"+{defChange}" : "";
            expGainText.text = (Arty.CurrentLevelExp + expGain).ToString();
            needExpText.text = needExp.ToString();
            
            // 슬라이더 트위닝 준비
            prevTargetValue = currentTargetValue;
            nextTargetValue = afterLevel + (float)currentLevelExp / needExp;
        }
        
        private void Update()
        {
            if (Mathf.Approximately(currentTargetValue, nextTargetValue))
                return;

            if (currentTargetValue < nextTargetValue)
            {
                IncreaseSlider();
            }
            else
            {
                DecreaseSlider();
            }
        }

        private void IncreaseSlider()
        {
            int currentLevel = Mathf.FloorToInt(currentTargetValue);
            float currentSliderValue = currentTargetValue - currentLevel;
            
            int nextLevel = Mathf.FloorToInt(nextTargetValue);
            float nextSliderValue = nextTargetValue - nextLevel;

            currentSliderValue += SLIDER_SPEED * Time.deltaTime;
            if (currentLevel == nextLevel)
            {
                if (currentSliderValue >= nextSliderValue)
                {
                    currentTargetValue = nextTargetValue;
                }
                else
                {
                    currentTargetValue = currentLevel + currentSliderValue;
                }

                expSlider.value = currentTargetValue - currentLevel;
                return;
            }
            
            // currentLevel < nextLevel
            if (currentSliderValue >= 1f)
            {
                currentLevel = nextLevel;
                currentSliderValue -= 1f;
            }
            
            currentTargetValue = currentLevel + currentSliderValue;
            expSlider.value = currentSliderValue;
        }
        
        private void DecreaseSlider()
        {
            int currentLevel = Mathf.FloorToInt(currentTargetValue);
            float currentSliderValue = currentTargetValue - currentLevel;
            
            int nextLevel = Mathf.FloorToInt(nextTargetValue);
            float nextSliderValue = nextTargetValue - nextLevel;

            currentSliderValue -= SLIDER_SPEED * Time.deltaTime;
            if (currentLevel == nextLevel)
            {
                if (currentSliderValue <= nextSliderValue)
                {
                    currentTargetValue = nextTargetValue;
                }
                else
                {
                    currentTargetValue = currentLevel + currentSliderValue;
                }

                expSlider.value = currentTargetValue - currentLevel;
                return;
            }
            
            // currentLevel > nextLevel
            if (currentSliderValue <= 0f)
            {
                currentLevel = nextLevel;
                currentSliderValue += 1f;
            }
            
            currentTargetValue = currentLevel + currentSliderValue;
            expSlider.value = currentSliderValue;
        }
    }
}