using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.UI;
using Mathlife.ProjectL.Utils;
using TMPro;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Serialization;
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
        
        // Settings
        [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField]
        private float shellMaxSpeed = 15f;

        [SerializeField]
        private float gravityScale = 0.1f;

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

        private float currentFuel;
        private const float FUEL_CONSUME_SPEED = 50f;

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
            IsPlayer = enemy == null;
            arty = artyModel;
            spriteRenderer.sprite = IsPlayer ? arty.Sprite : arty.EnemySprite;
            spriteRenderer.flipX = !IsPlayer;
            clockWise = IsPlayer;

            ProjectToSurface();

            fireGuideArrow.Setup();
            fireGuideArrow.Off();

            maxHp = artyModel.GetMaxHp();
            CurrentHp = artyModel.GetMaxHp();
            hpBar.fillAmount = (float)CurrentHp / maxHp;
            hpText.text = $"{CurrentHp}<space=0.2em>/<space=0.2em>{maxHp}";
            levelText.text = $"Lv. {artyModel.levelRx.Value}";
            
            Ready = true;

            if (IsPlayer)
            {
                behaviorGraphAgent.enabled = false;
            }
            else
            {
                behaviorGraphAgent.SetVariableValue("Enemy Move Strategy", enemy!.moveStrategy);
                behaviorGraphAgent.SetVariableValue("Enemy Attack Targeting Strategy", enemy!.targetingStrategy);
            }
        }

        private void ProjectToSurface()
        {
            DestructibleTerrain.Inst.SnapToSurface(transform.position, Vector2.up, out Vector2 surfacePosition);
            transform.position = surfacePosition;
        }

        public void StartTurn(int turn)
        {
            Debug.Log($"Turn {turn} start.");
            HasTurn = true;
            currentFuel = arty.GetMobility();

            // Enable HUD
            fireGuideArrow.On();

            if (IsPlayer)
            {
                Presenter.Find<FireHUD>().Enable();
                Presenter.Find<MoveHUD>().Enable();
            }
            
            Presenter.Find<FuelHUD>().SetFuel(currentFuel, arty.GetMobility());
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

            if (DestructibleTerrain.Inst.InGround(transform.position))
            {
                verticalVelocity = 0f;
            }

            if (false == HasTurn)
                return;
            
            if (currentFuel <= 0f)
            {
                Presenter.Find<MoveHUD>().Disable();
                return;
            }
            
            Slide(MoveAxis);

            // 이동 후 중력 작용
            if (DestructibleTerrain.Inst.InGround(transform.position) == false)
            {
                ApplyGravity();
            }
        }

        private void ApplyGravity()
        {
            verticalVelocity += gravityScale * Physics2D.gravity.y * Time.deltaTime;
            Vector2 nextPosition = (Vector2)transform.position + verticalVelocity * Vector2.up;

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
            bool slideResult = DestructibleTerrain.Inst.Slide(transform.position, slideAmount, out Vector2 endPosition,
                out Vector2 normal,
                out Vector2 tangent);

            currentFuel -= Mathf.Abs(slideAmount) * FUEL_CONSUME_SPEED;
            currentFuel =  Mathf.Max(currentFuel, 0f);
            Presenter.Find<FuelHUD>().SetFuel(currentFuel, arty.GetMobility());
            
            if (false == slideResult)
            {
                transform.position = (Vector2)transform.position + slideAmount * prevTangent;
                return;
            }

            clockWise = axis > 0f;

            transform.position = endPosition;
            prevNormal = normal;
            prevTangent = tangent;
            UpdateRotation();
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