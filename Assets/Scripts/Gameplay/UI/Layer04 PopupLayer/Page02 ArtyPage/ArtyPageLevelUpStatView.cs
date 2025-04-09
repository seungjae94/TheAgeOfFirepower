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

        private long prevExpGain = 0L;
        private long currentExpGain = 0L;
        private float targetSliderValue = 0f;
        private bool dirty = false;
        
        public override void Draw()
        {
            base.Draw();

            prevExpGain = 0L;
            dirty = false;
            
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
            expGainText.text = expGain.ToString();
            needExpText.text = needExp.ToString();
            
            // 슬라이더 트위닝 준비
            prevExpGain = currentExpGain;
            currentExpGain = expGain;
            
            targetSliderValue = (float) currentLevelExp / needExp;
            dirty = true;
        }
        
        private void Update()
        {
            if (dirty == false)
                return;

            if (prevExpGain == currentExpGain)
            {
                dirty = false;
                return;
            }

            float currentSliderValue = expSlider.value;

            if (Mathf.Approximately(currentSliderValue, targetSliderValue))
            {
                dirty = false;
                return;
            }
            
            // 증가
            if (prevExpGain < currentExpGain)
            {
                IncreaseSlider();
            }
            // 감소
            else if (prevExpGain > currentExpGain)
            {
                DecreaseSlider();
            }
        }

        private void IncreaseSlider()
        {
            float currentSliderValue = expSlider.value;
            
            // 현재 값이 더 작다
            if (currentSliderValue < targetSliderValue)
            {
                currentSliderValue += SLIDER_SPEED * Time.deltaTime;

                if (currentSliderValue >= targetSliderValue)
                {
                    // 도달 완료
                    dirty = false;
                    currentSliderValue = targetSliderValue;
                }
            }
            // 현재 값이 더 크다
            else
            {
                currentSliderValue += SLIDER_SPEED * Time.deltaTime;

                if (currentSliderValue >= 1f)
                {
                    currentSliderValue -= 1f;
                        
                    if (currentSliderValue >= targetSliderValue)
                    {
                        // 도달 완료
                        dirty = false;
                        currentSliderValue = targetSliderValue;
                    }
                }
            }
            
            expSlider.value = currentSliderValue;
        }
        
        private void DecreaseSlider()
        {
            float currentSliderValue = expSlider.value;
            
            // 현재 값이 더 크다
            if (currentSliderValue > targetSliderValue)
            {
                currentSliderValue -= SLIDER_SPEED * Time.deltaTime;

                if (currentSliderValue <= targetSliderValue)
                {
                    // 도달 완료
                    dirty = false;
                    currentSliderValue = targetSliderValue;
                }
            }
            // 현재 값이 더 작다
            else
            {
                currentSliderValue -= SLIDER_SPEED * Time.deltaTime;

                if (currentSliderValue <= 0f)
                {
                    currentSliderValue += 1f;
                        
                    if (currentSliderValue <= targetSliderValue)
                    {
                        // 도달 완료
                        dirty = false;
                        currentSliderValue = targetSliderValue;
                    }
                }
            }
            
            expSlider.value = currentSliderValue;
        }
    }
}