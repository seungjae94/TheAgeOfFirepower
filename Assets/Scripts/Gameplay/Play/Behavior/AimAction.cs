using Mathlife.ProjectL.Gameplay.Play;
using System;
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
        
        protected override Status OnStart()
        {
            return Status.Running;
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