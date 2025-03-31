using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Gameplay.Play;
using Mathlife.ProjectL.Gameplay.UI;
using Sirenix.Utilities;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class PlaySceneGameMode : GameMode<PlaySceneGameMode>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;

        [SerializeField]
        private new Camera camera;

        private Vector3 mousePosition = Vector3.zero;
        private readonly int circleRadiusPx = 12;

        public override async UniTask InitializeScene(IProgress<float> progress)
        {
            // 0. 게임 모드 공통 초기화 로직 수행
            await base.InitializeScene(progress);
            progress.Report(0.1f);

            // 1. 모든 UI 닫아놓기
            PlayCanvas.Inst.DeactivateAllPresenters();
            progress.Report(0.3f);

            // 2. 필요한 데이터 가져오기

            // 3. 플레이 준비
            DestructibleTerrain.Inst.GenerateTerrain();

            var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            
            var terrainLoadTasks = new List<UniTask>();
            foreach (var player in players)
            {
                var task = UniTask
                    .WaitUntil(player,
                        (p) => DestructibleTerrain.Inst.ProjectDownToSurface(p.transform.position, out Vector2 _));
                terrainLoadTasks.Add(task);
            }

            await UniTask.WhenAll(terrainLoadTasks);

            players.ForEach(player => player.Ready());

            // 4. HUD 열기
            //Presenter.Find<HomePage>().Open();
            progress.Report(0.6f);

            // 5. 딜레이
            await UniTask.Delay(100);
            progress.Report(1.0f);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 5f));
                DestructibleTerrain.Inst.Paint(mousePosition, Shape.Circle(circleRadiusPx), Color.clear);
            }
        }
    }
}