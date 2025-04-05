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
                    keySelector = player => (int)Vector3.SqrMagnitude(Agent.Value.transform.position - player.transform.position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            target = PlaySceneGameMode.Inst.AlivePlayers
                .OrderBy(keySelector)
                .FirstOrDefault();
            
            Debug.Log($"Target is {target.Description}");
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}