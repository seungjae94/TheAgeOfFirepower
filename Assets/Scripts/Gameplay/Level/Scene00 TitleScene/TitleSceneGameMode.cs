using System;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class TitleSceneGameMode : GameMode<TitleSceneGameMode>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;

        [SerializeField]
        private AudioClip titleBGM;
        
        [SerializeField]
        Button gameStartButton;

        CanvasGroup gameStartButtonCanvasGroup;

        void Awake()
        {
            gameStartButtonCanvasGroup = gameStartButton.GetComponent<CanvasGroup>();
        }

        void Start()
        {
            gameStartButtonCanvasGroup.Hide();
        }
        
        public override async UniTask InitializeScene(IProgress<float> progress)
        {
            await base.InitializeScene(progress);
            
            // 이 시점에선 게임이 초기화 되어 있다고 보장할 수 있음
            // 이미 볼륨이 설정된 상태임
            AudioManager.Inst.PlayBGM(titleBGM);
            
            gameStartButton.OnClickAsObservable()
                .Subscribe(OnClickGameStartButton)
                .AddTo(gameObject);
            gameStartButtonCanvasGroup.Show();
            
            progress.Report(1f);
        }

        private void OnClickGameStartButton(Unit _)
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            GameManager.Inst.ChangeScene(SceneNames.LOBBY_SCENE)
                .Forget();
        }
    }
}