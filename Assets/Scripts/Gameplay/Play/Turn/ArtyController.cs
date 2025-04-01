using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class ArtyController : MonoBehaviour
    {
        // Serialize Field
        
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private float speed = 5f;

        [SerializeField]
        private GameObject testShellPrefab;

        [SerializeField]
        private float testFireAngle = 30;

        [SerializeField]
        private float testFireSpeed = 1f;
        
#if UNITY_EDITOR
        [SerializeField]
        private bool drawTangentNormal = false;
#endif

        // Field
        public bool IsPlayer { get; private set; } = true;

        private ArtyModel arty = null;
        
        private bool onGround = true;
        
        public void Setup(ArtyModel playerArty)
        {
            if (playerArty == null)
                return;
            
            IsPlayer = true;
            arty = playerArty;

            spriteRenderer.sprite = arty.Sprite;
            spriteRenderer.flipX = false;

            ProjectToSurface();
        }
        
        public void Setup(Enemy enemy)
        {
            if (enemy == null)
                return;
            
            IsPlayer = false;
            arty = new ArtyModel(enemy.artyGameData, enemy.level, 0L);
            
            spriteRenderer.sprite = arty?.EnemySprite;
            spriteRenderer.flipX = true;
            
            ProjectToSurface();
        }

        public void Setup(DevelopArtyData developArty)
        {
            if (developArty == null)
                return;
            
            IsPlayer = developArty.isPlayer;
            arty = new ArtyModel(developArty.artyGameData, developArty.level, 0L);
            
            spriteRenderer.sprite = IsPlayer ? arty.Sprite : arty.EnemySprite;
            spriteRenderer.flipX = !IsPlayer;
            
            ProjectToSurface();
        }

        private void ProjectToSurface()
        {
            DestructibleTerrain.Inst.ProjectToSurface(transform.position, Vector2.up, out Vector2 surfacePosition);
            transform.position = surfacePosition;
        }

        // /// <summary>
        // /// 이동이 가능한지 체크
        // /// </summary>
        // /// <returns>
        // /// 다음 단계를 수행할 수 있는지 여부
        // /// </returns>
        // public bool MovabilityTest()
        // {
        //     bool thisFrameOnGround = DestructibleTerrain.Inst.InGround(transform.position);
        //
        //     // 임시로 가짜 중력 적용
        //     if (thisFrameOnGround == false)
        //     {
        //         DestructibleTerrain.Inst.ProjectDownToSurface(transform.position, out Vector2 surfacePosition);
        //         transform.position = surfacePosition;
        //     }
        //
        //     return true;
        // }

        public void Slide(float axis)
        {
            float slideAmount = axis * speed * Time.deltaTime;

            bool slideResult = DestructibleTerrain.Inst.Slide(transform.position, slideAmount, out Vector2 endPosition,
                out Vector2 normal,
                out Vector2 tangent);

            if (false == slideResult)
            {
                Vector2 nextPosition = transform.position + slideAmount * Vector3.right;

                if (DestructibleTerrain.Inst.InGround(nextPosition))
                {
                    DestructibleTerrain.Inst.ProjectToSurface(nextPosition, Vector3.up, out nextPosition);
                }
                // TODO: 중력 처리

                transform.position = nextPosition;
                Debug.Log("slide failed.");

                return;
            }

            transform.position = endPosition;

            float angle;
            if (axis > 0f)
            {
                spriteRenderer.flipX = false;
                angle = Vector3.SignedAngle(Vector3.right, tangent, Vector3.forward);
            }
            else
            {
                spriteRenderer.flipX = true;
                angle = Vector3.SignedAngle(Vector3.left, tangent, Vector3.forward);
            }

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

#if UNITY_EDITOR
            DrawTangentNormal(endPosition, tangent, normal);
#endif
        }

        public void Fire(float angle)
        {
            GameObject shellGameObject = Instantiate(testShellPrefab);
            shellGameObject.transform.position = transform.position + 0.4f * transform.up;
                
            IShell shell = shellGameObject.GetComponent<IShell>();
            shell.Init(GameState.Inst.gameDataLoader.GetShellData(0));
                
            Vector2 shellVelocity = new Vector2(
                Mathf.Cos(testFireAngle *  Mathf.Deg2Rad), 
                Mathf.Sin(testFireAngle * Mathf.Deg2Rad)) * testFireSpeed;
            shell.Fire(shellVelocity);
        }

#if UNITY_EDITOR
        private void DrawTangentNormal(Vector3 position, Vector3 tangent, Vector3 normal)
        {
            if (drawTangentNormal == false)
                return;

            DebugLineRenderer.Inst.DrawLine(position, position + tangent, Color.red, 0.01f);
            DebugLineRenderer.Inst.DrawLine(position, position + normal, Color.green, 0.01f);
        }
#endif
    }
}