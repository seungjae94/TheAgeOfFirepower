using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    // 공습 신호탄
    public class AirStrikeShell : ShellBase
    {
        private const int CHILDREN_COUNT = 3;
        private const float X_INTERVAL = 1.5f;
        private const float CHILD_SPEED = 5f;
        
        [SerializeField]
        private GameObject markerPrefab;
        
        [SerializeField]
        private GameObject childShellPrefab;

        [SerializeField]
        private AudioClip flareSound;
        
        [SerializeField]
        private AudioClip bomberSound;
        
        // Field
        private bool firstTouch = false;
        private readonly List<AirStrikeChildShell> children = new();

        // Method
        
        // Event Func
        private void Update()
        {
            if (firstTouch || ShouldBeDestroyed)
            {
                return;
            }
            
            if (false == DestructibleTerrain.Inst.InFairArea(transform.position))
            {
                ShouldBeDestroyed = true;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (firstTouch)
                return;
            
            // 지형과 충돌할 때만 처리
            if (false == IsCollisionWithTerrain(other))
            {
                return;
            }
            
            firstTouch = true;
            HideBody();
            
            OnTouchTerrain(other).Forget();
        }

        private async UniTaskVoid OnTouchTerrain(Collision2D other)
        {
            GameObject marker = Instantiate(markerPrefab);

            Vector2 contactPoint = GetContantPoint(other);
            marker.transform.position = new Vector3(contactPoint.x, contactPoint.y, -1f);

            var flareSource = AudioManager.Inst.BorrowAudioSource();
            flareSource.PlayOneShot(flareSound);
            
            await UniTask.Delay(500);
            
            // 카메라를 먼저 올리고
            PlaySceneCamera.Inst.SetTracking(new Vector3(transform.position.x, DestructibleTerrain.Inst.MapHeight + 10f));

            await UniTask.Delay(250);
            
            // 폭격기 사운드를 재생한 뒤
            AudioManager.Inst.PlayOneShotOnAudioPool(bomberSound).Forget();
            
            await UniTask.Delay(750);
            
            // 공습탄 스폰
            float baseX = transform.position.x - X_INTERVAL;
            for (int i = 0; i < CHILDREN_COUNT; i++)
            {
                float x = baseX + X_INTERVAL * i;
                
                GameObject shellGameObject = Instantiate(childShellPrefab);
                shellGameObject.transform.position = new Vector3(x, DestructibleTerrain.Inst.MapHeight + 10f);
                
                if (i == 1)
                    PlaySceneCamera.Inst.SetTracking(shellGameObject.transform);

                AirStrikeChildShell shell = shellGameObject.GetComponent<AirStrikeChildShell>();
                shell.Init(firer);
                shell.Fire(Vector2.down * CHILD_SPEED);
                
                children.Add(shell);
            }

            await UniTask.WaitUntil(children, list => list.All(c => c.ShouldBeDestroyed));

            foreach (var child in children)
            {
                Destroy(child.gameObject);
            }
            
            Destroy(marker);
            AudioManager.Inst.ReturnAudioSource(flareSource);
            
            ShouldBeDestroyed = true;
        }
    }
}