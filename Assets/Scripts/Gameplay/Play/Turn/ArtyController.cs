using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class ArtyController : MonoBehaviour
    {
        // Component & Children
        
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private FireGuideArrowRenderer fireGuideArrow;
        
        // Settings
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
        
        public bool Ready { get; private set; } = false;
        public bool HasTurn { get; private set; } = false;
        public bool IsPlayer { get; private set; } = true;

        private bool clockWise = true;
        private float verticalVelocity;
        private Vector2 prevNormal;
        private Vector2 prevTangent;
        private float fireAngle;
        private int firePower;

        public Vector2 Tangent => prevTangent;

        private ArtyModel arty = null;
        
        public void Setup(ArtyModel playerArty)
        {
            SetupInternal(true, playerArty);
        }
        
        public void Setup(Enemy enemy)
        {
            SetupInternal(false, 
                new ArtyModel(enemy.artyGameData, enemy.level, 0L));
        }

        public void Setup(DevelopArtyData developArty)
        {
            SetupInternal(developArty.isPlayer, 
                new ArtyModel(developArty.artyGameData, developArty.level, 0L));
        }

        private void SetupInternal(bool isPlayer, ArtyModel artyModel)
        {
            IsPlayer = isPlayer;
            arty = artyModel;
            spriteRenderer.sprite = IsPlayer ? arty.Sprite : arty.EnemySprite;
            spriteRenderer.flipX = !IsPlayer;
            clockWise = IsPlayer;

            ProjectToSurface();

            Ready = true;
            
            fireGuideArrow = GetComponentInChildren<FireGuideArrowRenderer>(true);
            fireGuideArrow.Setup();
            fireGuideArrow.Off();
        }

        private void ProjectToSurface()
        {
            DestructibleTerrain.Inst.SnapToSurface(transform.position, Vector2.up, out Vector2 surfacePosition);
            transform.position = surfacePosition;
        }

        public void StartTurn(int turn)
        {
            Debug.Log($"Turn {turn} start!");
            
            HasTurn = true;
            fireGuideArrow.On();
        }

        private void Update()
        {
            if (Ready == false)
                return;
            
            // 턴과 상관 없이 항상 중력 작용
            if (DestructibleTerrain.Inst.InGround(transform.position) == false)
            {
                ApplyGravity();
                return;
            }
            
            if (false == HasTurn)
                return;
            
            // TODO: InputSystem 통합
            float axis = Input.GetAxisRaw("Horizontal");
            Slide(axis);
            
            // 이동 후 중력 작용
            if (DestructibleTerrain.Inst.InGround(transform.position) == false)
            {
                ApplyGravity();
            }
        }

        private void ApplyGravity()
        {
            verticalVelocity += Physics2D.gravity.y * Time.deltaTime;
            Vector2 nextPosition = (Vector2) transform.position + verticalVelocity * Vector2.up;

            if (DestructibleTerrain.Inst.InGround(nextPosition))
            {
                DestructibleTerrain.Inst.VerticalSnapToSurface(transform.position, out nextPosition);
                
                DestructibleTerrain.Inst.ExtractNormalTangent(nextPosition, out prevNormal, out prevTangent);
                
                if (clockWise == false)
                    prevTangent = -prevTangent;
            }
            
            transform.position = nextPosition;

            UpdateRotation();
        }

        private void UpdateRotation()
        {
            spriteRenderer.flipX = !clockWise;
            float angle = clockWise ? Vector3.SignedAngle(Vector3.right, prevTangent, Vector3.forward) : Vector3.SignedAngle(Vector3.left, prevTangent, Vector3.forward);  
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            
            SetFireAngle(fireAngle);
            
            DrawTangentNormal();
        }

        private void Slide(float axis)
        {
            if (axis == 0f)
                return;
            
            float slideAmount = axis * speed * Time.deltaTime;

            bool slideResult = DestructibleTerrain.Inst.Slide(transform.position, slideAmount, out Vector2 endPosition,
                out Vector2 normal,
                out Vector2 tangent);

            if (false == slideResult)
            {
                transform.position= (Vector2)transform.position + slideAmount * prevTangent;
                return;
            }

            clockWise = axis > 0f;
            
            transform.position = endPosition;
            prevNormal = normal;
            prevTangent = tangent;
            UpdateRotation();
        }
        
        public void SetFireAngle(float angle)
        {
            fireAngle = angle;
            
            if (clockWise)
                fireGuideArrow.SetAngle(angle);
            else
                fireGuideArrow.SetAngle(180f - angle);
        }

        public void SetFirePower(int power)
        {
            firePower = power;
            fireGuideArrow.SetPower(power);
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
        private void DrawTangentNormal()
        {
            if (drawTangentNormal == false)
                return;

            DebugLineRenderer.Inst.DrawLine((Vector2) transform.position, (Vector2) transform.position + prevTangent, Color.red, 0.01f);
            DebugLineRenderer.Inst.DrawLine((Vector2) transform.position, (Vector2) transform.position + prevNormal, Color.green, 0.01f);
        }
#endif
    }
}