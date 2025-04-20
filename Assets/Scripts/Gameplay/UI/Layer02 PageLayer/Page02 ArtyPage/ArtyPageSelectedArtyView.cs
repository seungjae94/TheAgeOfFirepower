using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.UI;
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
        [SerializeField]
        private Image portrait;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private TextMeshProUGUI levelText;

        [SerializeField]
        private Slider expSlider;

        [SerializeField]
        private TextMeshProUGUI currentLevelExpText;

        [SerializeField]
        private TextMeshProUGUI needExpText;

        [SerializeField]
        private TextMeshProUGUI maxHpText;

        [SerializeField]
        private TextMeshProUGUI mobilityText;

        [SerializeField]
        private TextMeshProUGUI atkText;

        [SerializeField]
        private TextMeshProUGUI defText;

        [SerializeField]
        private Image shellIcon;
        
        [SerializeField]
        private TextMeshProUGUI shellNameText;
        
        [SerializeField]
        private TextMeshProUGUI shellDescriptionText;
        
        [SerializeField]
        private ArtyPageMechPartSlot[] mechPartSlots;

        [SerializeField]
        private Button levelUpButton;
        
        // Field
        private readonly CompositeDisposable disposables = new();
        private readonly CompositeDisposable characterDisposables = new();

        // Lifetime

        public override void Draw()
        {
            base.Draw();

            ArtyPage.selectedArtyIndexRx
                .DistinctUntilChanged()
                .Subscribe(index => SubscribeArty())
                .AddTo(disposables);

            levelUpButton.OnClickAsObservable()
                .Subscribe(_ => Presenter.Find<ArtyPageLevelUpPopup>().OpenWithAnimation().Forget())
                .AddTo(disposables);

            SubscribeArty();
        }

        public override void Clear()
        {
            base.Clear();

            disposables.Clear();
        }

        void OnDestroy()
        {
            disposables.Dispose();
            characterDisposables.Dispose();
        }

        public void SubscribeArty()
        {
            characterDisposables.Clear();
            
            ArtyModel arty = ArtyPage.SelectedArty;

            if (arty == null)
            {
                MyDebug.LogError("[ArtyPageSelectedArtyView] 선택된 화포가 없습니다.");
                throw new ArtyPageNoArtySelectedException("[ArtyPageSelectedArtyView] 선택된 화포가 없습니다.");
            }

            arty.ObserveEveryValueChanged(arty => arty.GetMaxHp())
                .DistinctUntilChanged()
                .Subscribe(value => UpdateView(arty))
                .AddTo(characterDisposables);
            
            arty.ObserveEveryValueChanged(arty => arty.GetAtk())
                .DistinctUntilChanged()
                .Subscribe(value => UpdateView(arty))
                .AddTo(characterDisposables);
            
            arty.ObserveEveryValueChanged(arty => arty.GetDef())
                .DistinctUntilChanged()
                .Subscribe(value => UpdateView(arty))
                .AddTo(characterDisposables);
            
            arty.ObserveEveryValueChanged(arty => arty.GetMobility())
                .DistinctUntilChanged()
                .Subscribe(value => UpdateView(arty))
                .AddTo(characterDisposables);
            
            arty.ObserveEveryValueChanged(arty => arty.totalExpRx.Value)
                .DistinctUntilChanged()
                .Subscribe(value => UpdateView(arty))
                .AddTo(characterDisposables);
            
            UpdateView(arty);
            
            RedrawSlots();
        }

        private void UpdateView(ArtyModel arty)
        {
            portrait.sprite = arty.Sprite;
            nameText.text = arty.DisplayName;
            levelText.text = arty.levelRx.Value.ToString();

            expSlider.value = (float)arty.CurrentLevelExp / arty.NeedExp;
            currentLevelExpText.text = arty.CurrentLevelExp.ToString();
            needExpText.text = arty.NeedExp.ToString();

            maxHpText.text = arty.GetMaxHp().ToString();
            mobilityText.text = arty.GetMobility().ToString();
            atkText.text = arty.GetAtk().ToString();
            defText.text = arty.GetDef().ToString();

            shellIcon.sprite = arty.Shell.icon;
            shellNameText.text = $"포탄 - {arty.Shell.displayName}";
            shellDescriptionText.text = arty.Shell.description;
        }
        

        private void RedrawSlots()
        {
            for (int i = 0;  i < mechPartSlots.Length; ++i)
            {
                var mechPartSlot = mechPartSlots[i];
                mechPartSlot.Clear();
                mechPartSlot.Setup((EMechPartType)i);
                mechPartSlot.Draw();
            }
        }
    }
}