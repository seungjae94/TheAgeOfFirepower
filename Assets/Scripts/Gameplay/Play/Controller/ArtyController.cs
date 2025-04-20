using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.UI;
using Mathlife.ProjectL.Utils;
using TMPro;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class ArtyController : MonoBehaviour
    {
        private const int DOUBLE_FIRE_DELAY_MS = 1000;

        // Component & Children
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private new Collider2D collider;

        [SerializeField]
        private BehaviorGraphAgent behaviorGraphAgent;

        [FormerlySerializedAs("fireGuideArrow")]
        [SerializeField]
        private FireVelocityRenderer fireVelocityRenderer;

        [SerializeField]
        private GameObject battlerCanvasGameObject;

        [SerializeField]
        private SlicedFilledImage hpBar;

        [SerializeField]
        private TextMeshProUGUI hpText;

        [SerializeField]
        private TextMeshProUGUI levelText;

        [SerializeField]
        private Image turnMarker;

        [SerializeField]
        private Sprite playerTurnSprite;

        [SerializeField]
        private Sprite enemyTurnSprite;

        [FormerlySerializedAs("refuelParticlePrefab")]
        [SerializeField]
        private GameObject refuelVFXPrefab;

        [FormerlySerializedAs("repairParticlePrefab")]
        [SerializeField]
        private GameObject repairVFXPrefab;

        [FormerlySerializedAs("doubleFireParticlePrefab")]
        [SerializeField]
        private GameObject doubleFireVFXPrefab;

        [SerializeField]
        private GameObject destroyVFXPrefab;

        [SerializeField]
        private GameObject smokeVFXPrefab;

        [SerializeField]
        private AudioClip repairSound;
        
        [SerializeField]
        private AudioClip refuelSound;
        
        [SerializeField]
        private AudioClip doubleFireSound;

        // Settings
        [SerializeField]
        private float moveSpeed = 5f;

        private bool slidedOnPrevFrame = false;
        private bool slidedOnCurFrame = false;
        
        public readonly float shellMaxSpeed = 15f;

        private static float gravityScale = 0.05f;

#if UNITY_EDITOR
        [SerializeField]
        private bool drawTangentNormal;
#endif

        // Field
        public Vector3 FirePoint => fireVelocityRenderer.transform.position;

        private GameObject doubleFireParticleInstance;
        private int fireChance = 1;

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
            private set => firePower = Mathf.Clamp(value, 10, 100);
        }

        public float CurrentFuel { get; private set; }

        private const float FUEL_CONSUME_SPEED = 20f; // 실제 서비스용
        //private const float FUEL_CONSUME_SPEED = 1f; // 이동 테스트용

        public bool Ready { get; private set; }
        public bool HasTurn { get; private set; }
        public bool IsPlayer { get; private set; } = true;

        private int maxHp = 100;
        public int CurrentHp { get; private set; } = 100;
        public int ThreatLevel => Model.GetThreatLevel();

        public string Description => $"{Model.DisplayName}(Lv. {Model.levelRx.Value})";

        private bool interactable = false;
        private bool clockWise = true;
        private float verticalVelocity;
        private Vector2 prevNormal;
        private Vector2 prevTangent;

        public Vector2 Tangent => prevTangent;

        public ArtyModel Model { get; private set; }

        public void Setup(ArtyModel artyModel, Enemy enemy)
        {
            // 데이터 세팅
            IsPlayer = enemy == null;
            Model = artyModel;

            // 렌더링 세팅
            spriteRenderer.sprite = IsPlayer ? Model.Sprite : Model.EnemySprite;
            spriteRenderer.flipX = !IsPlayer;
            clockWise = IsPlayer;

            // 물리 세팅
            DestructibleTerrain.Inst.VerticalSnapToSurface(transform.position, out Vector2 surfacePosition);
            transform.position = surfacePosition;

            bool extractResult =
                DestructibleTerrain.Inst.ExtractNormalTangent(surfacePosition, out Vector2 extNormal,
                    out Vector2 extTangent);

            if (extractResult)
            {
                prevNormal = extNormal;
                prevTangent = clockWise ? extTangent : -extTangent;
            }

            UpdateRotation();

            // 상태 초기화
            maxHp = artyModel.GetMaxHp();
            CurrentHp = artyModel.GetMaxHp();

            // UI 및 컴포넌트 세팅
            hpBar.fillAmount = (float)CurrentHp / maxHp;
            hpText.text = $"{CurrentHp}<space=0.2em>/<space=0.2em>{maxHp}";
            levelText.text = $"Lv. {artyModel.levelRx.Value}";
            turnMarker.enabled = false;

            if (IsPlayer)
            {
                behaviorGraphAgent.enabled = false;
            }
            else
            {
                behaviorGraphAgent.SetVariableValue("Enemy Move Strategy", enemy!.moveStrategy);
                behaviorGraphAgent.SetVariableValue("Enemy Attack Targeting Strategy", enemy!.targetingStrategy);
            }

            fireVelocityRenderer.Setup();
            TurnOffFireResultDrawing(false);
            SetFireAngle(0);
            SetFirePower(50);
            
            // 준비 완료
            Ready = true;
        }

        public void StartTurn(int turn)
        {
            MyDebug.Log($"Turn {turn} start.");
            HasTurn = true;

            // 상태 초기화
            MoveAxis = 0f;
            CurrentFuel = Model.GetMobility();
            fireChance = 1;

            // Enable UI
            interactable = true;
            turnMarker.sprite = IsPlayer ? playerTurnSprite : enemyTurnSprite;
            turnMarker.enabled = true;

            if (IsPlayer)
            {
                Presenter.Find<GaugeHUD>().Enable();
                Presenter.Find<MoveHUD>().Enable();
                Presenter.Find<ItemHUD>().Enable();
                
                UniTask.DelayFrame(3)
                    .ContinueWith(UpdateRotation)
                    .ContinueWith(() => TurnOnFireResultDrawing(true))
                    .Forget();
            }
            else
            {
                behaviorGraphAgent.Restart();
            }

            if (IsPlayer)
                Presenter.Find<GaugeHUD>().SetFuel(CurrentFuel, Model.GetMobility());

            // Camera Tracking Start
            PlaySceneCamera.Inst.SetTracking(transform);
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

            slidedOnCurFrame = false;
            
            TurnOffFireResultDrawing(true);

            // 0. 유효 범위 바깥으로 나간 경우 낙사
            if (DestructibleTerrain.Inst.InFairArea(transform.position) == false)
            {
                Skip();
                return;
            }

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

                bool extractResult = DestructibleTerrain.Inst.ExtractNormalTangent(surfacePosition,
                    out Vector2 extNormal, out Vector2 extTangent);

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
                // 움직이지 않을 때만 발사 결과 표시
                TurnOnFireResultDrawing(true);

                if (MoveAxis < 0f)
                {
                    SetDirection(false);
                }
                else if (MoveAxis > 0f)
                {
                    SetDirection(true);
                }

                return;
            }

            if (MoveAxis == 0f)
            {
                // 움직이지 않을 때만 발사 결과 표시
                TurnOnFireResultDrawing(true);
                return;
            }

            Slide(MoveAxis);
            PlaySceneCamera.Inst.SetTracking(transform);

            // 이동 후 중력 작용
            if (DestructibleTerrain.Inst.InGround(transform.position) == false)
            {
                FallFromAir();
            }
        }

        private void LateUpdate()
        {
            if (slidedOnPrevFrame && !slidedOnCurFrame)
            {
                AudioManager.Inst.StopSE(1); // 엔진 사운드
            }
            
            slidedOnPrevFrame = slidedOnCurFrame;
        }

        private void FallFromAir()
        {
            verticalVelocity += gravityScale * Physics2D.gravity.y * Time.deltaTime;

            verticalVelocity =
                Mathf.Min(verticalVelocity, -1.1f / DestructibleTerrain.Inst.PixelsPerUnit); // 최소 1.1 픽셀은 아래로 내려가야 한다.

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
                            MyDebug.Log(prevNormal);
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

            DrawOnlyIfTrajectory();
        }

        private void Slide(float axis)
        {
            if (axis == 0f)
                return;
            
            slidedOnCurFrame = true;

            // 이번 프레임에 처음 슬라이딩
            if (slidedOnPrevFrame == false)
            {
                AudioManager.Inst.PlaySE(ESoundEffectId.Engine, 1); // 엔진 사운드
            }

            float slideAmount = axis * moveSpeed * Time.deltaTime;
            SlideResult slideResult = DestructibleTerrain.Inst.Slide(transform.position, slideAmount,
                out Vector2 endPosition,
                out Vector2 normal,
                out Vector2 tangent);

            if (slideResult is SlideResult.ShortSpline or SlideResult.WrongSpline)
            {
                MyDebug.Log("슬라이드 실패");

                endPosition = (Vector2)transform.position + slideAmount * prevTangent;
                DestructibleTerrain.Inst.SnapToSurface(endPosition, prevNormal, out endPosition);
                DestructibleTerrain.Inst.ExtractNormalTangent(endPosition, out prevNormal, out prevTangent);
                transform.position = endPosition;
                ConsumeFuel(slideAmount);
                return;
            }

            // 유효 범위 바깥으로 나가는 움직임은 무시한다.
            if (DestructibleTerrain.Inst.InTerrain(endPosition) == false ||
                DestructibleTerrain.Inst.IsBoundary(endPosition))
            {
                MyDebug.Log("유효 범위 바깥으로 슬라이드 시도 차단");
                return;
            }

            if (float.IsNaN(endPosition.x) || float.IsNaN(endPosition.y) || float.IsNaN(normal.x) ||
                float.IsNaN(normal.y))
                MyDebug.Log("WR");

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
            CurrentFuel = Mathf.Max(CurrentFuel, 0f);
            
            if (IsPlayer)
                Presenter.Find<GaugeHUD>().SetFuel(CurrentFuel, Model.GetMobility());
        }

        public void Refuel(float amount)
        {
            AudioManager.Inst.PlayOneShotOnAudioPool(refuelSound).Forget();
            
            GameObject particleInstance = Instantiate(refuelVFXPrefab, spriteRenderer.transform);
            particleInstance.transform.localScale /= spriteRenderer.transform.localScale.x;
            DisposeVFX(particleInstance.GetComponent<ParticleSystem>()).Forget();

            CurrentFuel += Mathf.Abs(amount);
            
            if (IsPlayer)
                Presenter.Find<GaugeHUD>().SetFuel(CurrentFuel, Model.GetMobility());
        }

        public void Repair(float ratio)
        {
            AudioManager.Inst.PlayOneShotOnAudioPool(repairSound).Forget();
            
            GameObject particleInstance = Instantiate(repairVFXPrefab, spriteRenderer.transform);
            particleInstance.transform.localScale /= spriteRenderer.transform.localScale.x;
            DisposeVFX(particleInstance.GetComponent<ParticleSystem>()).Forget();

            int prevHp = CurrentHp;
            float finalHp = Mathf.Min(prevHp + maxHp * ratio, maxHp);
            CurrentHp = Mathf.CeilToInt(finalHp);

            DamageTextGenerator.Inst.Generate(this, CurrentHp - prevHp, true); // TODO: 힐은 초록색으로 표시
            hpText.text = $"{CurrentHp}<space=0.2em>/<space=0.2em>{maxHp}";
            DOTween.To(() => hpBar.fillAmount, (float v) => hpBar.fillAmount = v, (float)CurrentHp / maxHp, 0.25f);
        }

        private async UniTask DisposeVFX(ParticleSystem ps)
        {
            await UniTask.WaitWhile(ps.IsAlive);
            Destroy(ps.gameObject);
        }

        public void DoubleFireBuff()
        {
            AudioManager.Inst.PlaySE(doubleFireSound, 2);
            
            doubleFireParticleInstance = Instantiate(doubleFireVFXPrefab, spriteRenderer.transform);
            doubleFireParticleInstance.transform.localScale /= spriteRenderer.transform.localScale.x;
            fireChance = 2;
        }

        public void SetFireAngle(int angle)
        {
            FireAngle = angle;
            DrawFireResult();
        }

        public void SetFirePower(int power)
        {
            FirePower = power;
            DrawFireResult();
        }

        private Vector3 GetFireVelocity()
        {
            Vector2 direction = Quaternion.Euler(0, 0, clockWise ? FireAngle : -FireAngle) * prevTangent;
            return shellMaxSpeed * (0.1f + 0.9f * FirePower / 100f) * direction.normalized;
        }

        private void TurnOnFireResultDrawing(bool shouldHasTurn)
        {
            if (IsPlayer == false || interactable == false)
                return;

            if (shouldHasTurn && !HasTurn)
            {
                return;
            }
            
            if (GameState.Inst.gameSettingState.drawTrajectory.Value)
            {
                TrajectoryRenderer.Inst.On();
            }
            else
            {
                fireVelocityRenderer.On();
            }
        }

        private void TurnOffFireResultDrawing(bool shouldHasTurn)
        {
            if (shouldHasTurn && !HasTurn)
            {
                return;
            }

            if (GameState.Inst.gameSettingState.drawTrajectory.Value)
            {
                TrajectoryRenderer.Inst.Off();
            }
            else
            {
                fireVelocityRenderer.Off();
            }
        }

        private void DrawFireResult()
        {
            if (HasTurn == false)
                return;

            if (GameState.Inst.gameSettingState.drawTrajectory.Value)
            {
                DrawTrajectory();
            }
            else
            {
                fireVelocityRenderer.Draw(clockWise, FireAngle, FirePower);
            }
        }

        private void DrawOnlyIfTrajectory()
        {
            if (HasTurn == false)
                return;

            if (GameState.Inst.gameSettingState.drawTrajectory.Value)
            {
                DrawTrajectory();
            }
        }

        private void DrawTrajectory()
        {
            if (GameState.Inst.gameSettingState.drawTrajectory.Value == false)
                return;

            Vector3 velocity = GetFireVelocity();

            const int SAMPLING_INTERVAL = 3;
            int index = 1;
            
            List<Vector3> positions = new();
            positions.Add(FirePoint);

            Vector3 prevPosition = FirePoint;
            while (positions.Count < 300)
            {
                var position = prevPosition;
                position += velocity * Time.deltaTime;
                velocity += Physics.gravity * Time.deltaTime;

                if (DestructibleTerrain.Inst.InFairArea(position) == false
                    || DestructibleTerrain.Inst.InGround(position))
                {
                    positions.Add(position);
                    break;
                }
                
                if (index % SAMPLING_INTERVAL == 0)
                    positions.Add(position);

                prevPosition = position;
                ++index;
            }

            TrajectoryRenderer.Inst.Draw(positions);
        }

        public void Fire()
        {
            if (CurrentHp <= 0)
            {
                return;
            }

            AudioManager.Inst.PlaySE(ESoundEffectId.Fire);
            --fireChance;

            GameObject shellGameObject = Instantiate(Model.Shell.prefab);
            shellGameObject.transform.position = FirePoint;

            IShell shell = shellGameObject.GetComponent<IShell>();
            shell.Init(this);

            Vector2 shellVelocity = GetFireVelocity();
            shell.Fire(shellVelocity);

            // 카메라가 포탄을 추적하도록 변경
            PlaySceneCamera.Inst.SetTracking(shellGameObject.transform);

            // Disable UI and HUD
            DisableUIAndHUD();

            WaitUntilAllShellsExploded(shell)
                .ContinueWith(() => Destroy(shellGameObject))
                .Forget();
        }

        public void Skip()
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            
            HasTurn = false;
            DisableUIAndHUD();
        }

        private void DisableUIAndHUD()
        {
            interactable = false;
            
            // Disable UI and HUD
            TurnOffFireResultDrawing(false);
            Presenter.Find<GaugeHUD>().Disable();
            Presenter.Find<MoveHUD>().Disable();
            Presenter.Find<ItemHUD>().Disable();
        }

        public void EndTurn()
        {
            turnMarker.enabled = false;

            AudioManager.Inst.StopSE(2);
            if (doubleFireParticleInstance)
                Destroy(doubleFireParticleInstance);
        }

        private async UniTask WaitUntilAllShellsExploded(IShell rootShell)
        {
            await UniTask.WaitUntil(rootShell, root => root.ShouldBeDestroyed);

            if (fireChance > 0)
            {
                await UniTask.Delay(DOUBLE_FIRE_DELAY_MS);
                PlaySceneCamera.Inst.SetTracking(transform);
                await UniTask.Delay(DOUBLE_FIRE_DELAY_MS);
                Fire();
                return;
            }

            HasTurn = false;
        }

        public void Damage(float damage)
        {
            int finalDamage = Mathf.CeilToInt(100f * damage / (100f + Model.GetDef()));
            MyDebug.Log($"final damage: {finalDamage}");

            DamageTextGenerator.Inst.Generate(this, finalDamage, false);
            CurrentHp = Mathf.Max(0, CurrentHp - finalDamage);
            hpText.text = $"{CurrentHp}<space=0.2em>/<space=0.2em>{maxHp}";
            DOTween.To(() => hpBar.fillAmount, (float v) => hpBar.fillAmount = v, (float)CurrentHp / maxHp, 0.25f);

            if (CurrentHp == 0)
            {
                collider.enabled = false;
                PlayDestroyEffects().Forget();
            }
        }

        private async UniTaskVoid PlayDestroyEffects()
        {
            GameObject destroyParticleInstance = Instantiate(destroyVFXPrefab, spriteRenderer.transform);
            destroyParticleInstance.transform.localScale /= spriteRenderer.transform.localScale.x;

            Color grayColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);
            spriteRenderer.DOColor(grayColor, 0.5f);

            await DisposeVFX(destroyParticleInstance.GetComponent<ParticleSystem>());

            fireVelocityRenderer.Off();
            battlerCanvasGameObject.SetActive(false);

            GameObject smokeParticleInstance = Instantiate(smokeVFXPrefab, spriteRenderer.transform);
            smokeParticleInstance.transform.localScale /= spriteRenderer.transform.localScale.x;

            await UniTask.Delay(500);

            if (HasTurn)
                HasTurn = false;
        }
    }
}