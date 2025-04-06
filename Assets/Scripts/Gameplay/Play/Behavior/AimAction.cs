using Mathlife.ProjectL.Gameplay.Play;
using System;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace Mathlife.ProjectL.Gameplay.Play
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Aim", story: "[Agent] aims according to [Strategy]", category: "Action",
        id: "fa7fd8805744b1886308532a970622e2")]
    public partial class AimAction : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> Agent;

        [SerializeReference]
        public BlackboardVariable<AttackTargetingStrategy> Strategy;

        [SerializeReference]
        public BlackboardVariable<float> Accuracy;

        private float fireAngle;
        private float firePower;

        private ArtyController controller;
        private ArtyController target;

        protected override Status OnStart()
        {
            Func<ArtyController, int> keySelector = null;

            switch (Strategy.Value)
            {
                case AttackTargetingStrategy.LowestHpFirst:
                    keySelector = player => player.CurrentHp;
                    break;
                case AttackTargetingStrategy.HighestDamageFirst:
                    keySelector = player => int.MaxValue - player.ThreatLevel;
                    break;
                case AttackTargetingStrategy.NearestFirst:
                    keySelector = player =>
                        (int)Vector3.SqrMagnitude(Agent.Value.transform.position - player.transform.position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            controller = Agent.Value.GetComponent<ArtyController>();
            target = PlaySceneGameMode.Inst.AlivePlayers
                .OrderBy(keySelector)
                .FirstOrDefault();

            Debug.Log($"Target is {target.Description}");

            if (target.transform.position.x < controller.transform.position.x)
            {
                controller.SetDirection(false);
            }
            else
            {
                controller.SetDirection(true);
            }

            // 방향 설정
            controller.SetDirection(controller.transform.position.x < target.transform.position.x);

            // 1. 파워 100을 기준으로 물리적 theta 계산 (높, 낮)
            // 2. 바라보는 방향에 대한 상대적 theta 계산 (높, 낮)
            // -> case 1: 상대적 theta를 75도 보다 더 높혀야 닿는다 = 사거리 부족 = 파워를 50으로 낮추고 (이분 탐색) 1로 돌아간다.
            // -> case 2: 상대적 theta를 -15도 보다 더 낮춰야 닿는다 = 사거리 초과 = 이동 필요  
            // 3. 낮은 각도로 먼저 시뮬레이션. 중간에 장애물이 있을 경우 높은 각도로 발사. 

            int power = 70;
            float s = (0.1f + 0.9f * power / 100f) * controller.shellMaxSpeed;
            float g = Mathf.Abs(Physics2D.gravity.y);
            float d = Mathf.Abs(controller.transform.position.x - target.transform.position.x);
            float y = target.transform.position.y - controller.transform.position.y;
            
            float angleOfTangent = controller.GetDirection() ? Vector2.SignedAngle(Vector2.right, controller.Tangent) : -Vector2.SignedAngle(Vector2.left, controller.Tangent);
            
            // 사거리가 닿지 않는다.
            float det = Mathf.Pow(s, 4f) - g * (g * d * d + 2 * y * s * s);

            if (det < 0f)
            {
                controller.Skip();
                return Status.Success;
            }
            
            float thetaH = Mathf.Atan2((s * s + Mathf.Sqrt(det)), g * d) * Mathf.Rad2Deg;
            float thetaL = Mathf.Atan2((s * s - Mathf.Sqrt(det)), g * d) * Mathf.Rad2Deg;

            thetaH -= angleOfTangent;
            thetaL -= angleOfTangent;
            
            if ((thetaH < -15f || thetaH > 75f) && (thetaL > 75f || thetaL < -15f))
            {
                controller.Skip();
                return Status.Success;
            }

            if (thetaL >= -15f)
            {
                controller.SetFireAngle((int)thetaL);
            }
            else
            {
                controller.SetFireAngle((int)thetaH);
            }
            
            controller.SetFirePower(power);
            controller.Fire();
            return Status.Success;
        }

        protected override Status OnUpdate()
        {
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}