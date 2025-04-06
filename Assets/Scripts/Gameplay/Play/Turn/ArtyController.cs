using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.UI;
using Mathlife.ProjectL.Utils;
using TMPro;
using Unity.Behavior;
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

        [SerializeField]
        private BehaviorGraphAgent behaviorGraphAgent;
        
        [SerializeField]
        private FireGuideArrowRenderer fireGuideArrow;

        [SerializeField]
        private SlicedFilledImage hpBar;

        [SerializeField]
        private TextMeshProUGUI hpText;
        
        [SerializeField]
        private TextMeshProUGUI levelText;

        private Camera mainCamera;
        
        // Settings
        [SerializeField]
        private float moveSpeed = 5f;
        
        public readonly float shellMaxSpeed = 15f;
        
        private static float gravityScale = 0.05f;

        [SerializeField]
        private GameObject testShellPrefab;


#if UNITY_EDITOR
        [SerializeField]
        private bool drawTangentNormal;
#endif

        // Field
        private float moveAxis;
        public float MoveAxis
        {
            get => moveAxis;
            set => moveAxis = Mathf.Clamp(value, -1f, 1f);
        }

        private int fireAngle;

        public int FireAngle
        {
            get => fireAngle;
            set => fireAngle = Mathf.Clamp(value, -15, 75);
        }
        
        private int firePower = 50;

        public int FirePower
        {
            get => firePower;
            set => firePower = Mathf.Clamp(value, 1, 100);
        }

        public float CurrentFuel { get; private set; }
        
        private const float FUEL_CONSUME_SPEED = 50f; // 실제 서비스용
        //private const float FUEL_CONSUME_SPEED = 1f; // 이동 테스트용

        public bool Ready { get; private set; }
        public bool HasTurn { get; private set; }
        public bool IsPlayer { get; private set; } = true;
        
        private int maxHp = 100;
        public int CurrentHp { get; private set; } = 100;
        public int ThreatLevel => arty.GetThreatLevel();

        public string Description => $"{arty.DisplayName}(Lv. {arty.levelRx.Value})";
        
        private bool clockWise = true;
        private float verticalVelocity;
        private Vector2 prevNormal;
        private Vector2 prevTangent;

        public Vector2 Tangent => prevTangent;

        private ArtyModel arty;

        public void Setup(ArtyModel artyModel, Enemy enemy)
        {
            mainCamera = Cameras.Inst.MainCamera;
            
            // 데이터 세팅
            IsPlayer = enemy == null;
            arty = artyModel;
            
            // 렌더링 세팅
            spriteRenderer.sprite = IsPlayer ? arty.Sprite : arty.EnemySprite;
            spriteRenderer.flipX = !IsPlayer;
            clockWise = IsPlayer;

            // 물리 세팅
            prevTangent = IsPlayer ? Vector2.right : Vector2.left;
            prevNormal = Vector2.up;

            // UI 및 컴포넌트 세팅
            maxHp = artyModel.GetMaxHp();
            CurrentHp = artyModel.GetMaxHp();
            hpBar.fillAmount = (float)CurrentHp / maxHp;
            hpText.text = $"{CurrentHp}<space=0.2em>/<space=0.2em>{maxHp}";
            levelText.text = $"Lv. {artyModel.levelRx.Value}";
            
            if (IsPlayer)
            {
                behaviorGraphAgent.enabled = false;
            }
            else
            {
                behaviorGraphAgent.SetVariableValue("Enemy Move Strategy", enemy!.moveStrategy);
                behaviorGraphAgent.SetVariableValue("Enemy Attack Targeting Strategy", enemy!.targetingStrategy);
            }
            
            fireGuideArrow.Setup();
            fireGuideArrow.Off();
            
            // 준비 완료
            Ready = true;
        }

        public void StartTurn(int turn)
        {
            Debug.Log($"Turn {turn} start.");
            HasTurn = true;
            MoveAxis = 0f;
            CurrentFuel = arty.GetMobility();

            // Enable HUD
            fireGuideArrow.On();

            if (IsPlayer)
            {
                Presenter.Find<FireHUD>().Enable();
                Presenter.Find<MoveHUD>().Enable();
            }
            else
            {
                behaviorGraphAgent.Restart();
            }
            
            Presenter.Find<FuelHUD>().SetFuel(CurrentFuel, arty.GetMobility());
        }

        public bool GetDirection()
        {
            return clockWise;
        }
        
        public void SetDirection(bool clockWise)
        {
            if (clockWise != this.clockWise)
                prevTangent = -prevTangent;
            
            this.clockWise = clockWise;
            UpdateRotation();
        }

        private void Update()
        {
            // 준비가 끝난 뒤부터 물리 계산 시작 
            if (Ready == false)
                return;
            
            // 1. 공중에 떠 있는 경우 중력 적용
            if (DestructibleTerrain.Inst.InGround(transform.position) == false)
            {
                FallFromAir();
                return;
            }
            
            // 2. 땅 속에 박혀 있는 경우 일단 표면으로 올린다.
            if (DestructibleTerrain.Inst.OnSurface(transform.position) == false)
            {
                DestructibleTerrain.Inst.SnapToSurface(transform.position, prevNormal, out Vector2 surfacePosition);
                
                bool extractResult = DestructibleTerrain.Inst.ExtractNormalTangent(surfacePosition,  out Vector2 extNormal, out Vector2 extTangent);

                if (extractResult)
                {
                    prevNormal = extNormal;
                    prevTangent = clockWise ? extTangent : -extTangent;
                }
                
                transform.position = surfacePosition;
            }
            
            // 3. 표면에 붙어 있고 노멀이 0 이하인 경우 중력 적용
            if (prevNormal.y <= 0f)
            {
                FallFromGround();
                return;
            }
            
            // 턴과 상관 없이 항상 중력 작용
            // 자신의 턴일 때만 이동 가능
            if (false == HasTurn)
                return;
            
            if (CurrentFuel <= 0f)
            {
                Presenter.Find<MoveHUD>().Disable();
                return;
            }

            Slide(MoveAxis);

            // 이동 후 중력 작용
            if (DestructibleTerrain.Inst.InGround(transform.position) == false)
            {
                FallFromAir();
            }
        }

        private void FallFromAir()
        {
            verticalVelocity += gravityScale * Physics2D.gravity.y * Time.deltaTime;

            verticalVelocity = Mathf.Min(verticalVelocity, -1.1f / DestructibleTerrain.Inst.PixelsPerUnit); // 최소 1.1 픽셀은 아래로 내려가야 한다.
            
            Vector2 nextPosition = (Vector2)transform.position + verticalVelocity * Vector2.up;
            
            if (DestructibleTerrain.Inst.InGround(nextPosition))
            {
                DestructibleTerrain.Inst.VerticalSnapToSurface(nextPosition, out nextPosition);
                DestructibleTerrain.Inst.ExtractNormalTangent(nextPosition, out prevNormal, out prevTangent);
                if (clockWise == false)
                    prevTangent = -prevTangent;
                verticalVelocity = 0f;
            }
            
            transform.position = nextPosition;

            UpdateRotation();
        }

        private void FallFromGround()
        {
            float minimalDisplacement = 1.1f / DestructibleTerrain.Inst.PixelsPerUnit;
            verticalVelocity = -minimalDisplacement; // 최소 1.1 픽셀은 아래로 내려가야 한다.
            Vector2 nextPosition = (Vector2)transform.position + verticalVelocity * Vector2.up;

            int iter = 5;
            while (DestructibleTerrain.Inst.InGround(nextPosition))
            {
                nextPosition += verticalVelocity * Vector2.up;
                
                --iter;
                if (iter < 0)
                {
                    iter = 5;
                    nextPosition = (Vector2)transform.position + minimalDisplacement * prevNormal;
                    while (DestructibleTerrain.Inst.InGround(nextPosition))
                    {
                        nextPosition += minimalDisplacement * prevNormal;

                        --iter;
                        if (iter < 0)
                        {
                            Debug.Log(prevNormal);
                            throw new OverflowException("");
                        }
                    }
                    transform.position = nextPosition;
                    return;
                }
            }
            
            transform.position = nextPosition;
        }

        private void UpdateRotation()
        {
            spriteRenderer.flipX = !clockWise;
            float angle = clockWise
                ? Vector3.SignedAngle(Vector3.right, prevTangent, Vector3.forward)
                : Vector3.SignedAngle(Vector3.left, prevTangent, Vector3.forward);
            spriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            SetFireAngle(FireAngle);

            DrawTangentNormal();
        }

        private void Slide(float axis)
        {
            if (axis == 0f)
                return;

            float slideAmount = axis * moveSpeed * Time.deltaTime;

            // 범위 바깥으로 나가는 움직임은 무시한다.
            if (DestructibleTerrain.Inst.InTerrain((Vector2)transform.position + prevTangent * Mathf.Abs(slideAmount)) == false)
            {
                return;
            }
            
            SlideResult slideResult = DestructibleTerrain.Inst.Slide(transform.position, slideAmount, out Vector2 endPosition,
                out Vector2 normal,
                out Vector2 tangent);
            
            if (slideResult is SlideResult.ShortSpline or SlideResult.WrongSpline)
            {
                endPosition = (Vector2)transform.position + slideAmount * prevTangent;
                DestructibleTerrain.Inst.SnapToSurface(endPosition, prevNormal, out endPosition);
                DestructibleTerrain.Inst.ExtractNormalTangent(endPosition, out prevNormal, out prevTangent);
                transform.position = endPosition;
                ConsumeFuel(slideAmount);
                return;
            }
            
            if (float.IsNaN(endPosition.x) || float.IsNaN(endPosition.y) || float.IsNaN(normal.x) || float.IsNaN(normal.y))
                Debug.Log("WR");
            
            // 절벽 못올라가게 막기
            if (normal.y <= 0f && endPosition.y > transform.position.y)
            {
                return;
            }

            clockWise = axis > 0f;

            transform.position = endPosition;
            prevNormal = normal;
            prevTangent = tangent;
            UpdateRotation();
            ConsumeFuel(slideAmount);
        }

        private void ConsumeFuel(float amount)
        {
            CurrentFuel -= Mathf.Abs(amount) * FUEL_CONSUME_SPEED;
            CurrentFuel =  Mathf.Max(CurrentFuel, 0f);
            Presenter.Find<FuelHUD>().SetFuel(CurrentFuel, arty.GetMobility());
        }
        
        public void SetFireAngle(int angle)
        {
            FireAngle = angle;

            if (clockWise)
                fireGuideArrow.SetAngle(angle);
            else
                fireGuideArrow.SetAngle(180f - angle);
        }

        public void SetFirePower(int power)
        {
            FirePower = power;
            fireGuideArrow.SetPower(power);
        }

        public void Fire()
        {
            GameObject shellGameObject = Instantiate(testShellPrefab);
            shellGameObject.transform.position = fireGuideArrow.transform.position;

            IShell shell = shellGameObject.GetComponent<IShell>();
            shell.Init(arty, GameState.Inst.gameDataLoader.GetShellData(0));

            Vector2 shellVelocity = fireGuideArrow.GetVelocity() * shellMaxSpeed;
            shell.Fire(shellVelocity);

            // Disable HUD
            fireGuideArrow.Off();
            Presenter.Find<FireHUD>().Disable();
            Presenter.Find<MoveHUD>().Disable();

            WaitUntilAllShellsExploded(new() { shellGameObject }).Forget();
        }

        private async UniTaskVoid WaitUntilAllShellsExploded(List<GameObject> objects)
        {
            await UniTask.WaitUntil(objects, value => value.All(obj => obj == null));
            HasTurn = false;
        }

        public void Damage(float damage)
        {
            int finalDamage = Mathf.CeilToInt(100f *  damage / (100f + arty.GetDef()));
            
            DamageTextGenerator.Inst.Generate(this, finalDamage);
            CurrentHp = Mathf.Max(0, CurrentHp - finalDamage);
            hpText.text = $"{CurrentHp}<space=0.2em>/<space=0.2em>{maxHp}";
            DOTween.To(() => hpBar.fillAmount, (float v) => hpBar.fillAmount = v, (float)CurrentHp / maxHp, 0.25f);
        }

        private void LateUpdate()
        {
            if (HasTurn == false)
                return;
            
            AttractCamera();
        }

        private void AttractCamera()
        {
            var cameraPosition = transform.position + 2f * Vector3.down;
            mainCamera.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, mainCamera.transform.position.z);
        }
        
#if UNITY_EDITOR
        private void DrawTangentNormal()
        {
            if (drawTangentNormal == false)
                return;

            DebugLineRenderer.Inst.DrawLine((Vector2)transform.position, (Vector2)transform.position + prevTangent,
                Color.red, 0.01f);
            DebugLineRenderer.Inst.DrawLine((Vector2)transform.position, (Vector2)transform.position + prevNormal,
                Color.green, 0.01f);
        }
#endif
    }
}