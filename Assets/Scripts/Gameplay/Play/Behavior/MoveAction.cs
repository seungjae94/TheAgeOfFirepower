using Mathlife.ProjectL.Gameplay.Play;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace Mathlife.ProjectL.Gameplay.Play
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move", story: "[Agent] move according to [Strategy]", category: "Action",
        id: "4249e51cc56a5f54c3dbe97b9ab148e7")]
    public partial class MoveAction : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> Agent;

        [SerializeReference]
        public BlackboardVariable<MoveStrategy> Strategy;

        protected override Status OnStart()
        {
            var currentBattler = PlaySceneGameMode.Inst?.turnOwner;
            
            if (Agent.Value != currentBattler?.gameObject)
            {
                return Status.Failure;
            }

            if (currentBattler?.HasTurn == false)
            {
                return Status.Failure;
            }
            
            Debug.Log("Move Action Started!");
            // TODO: 목적지 계산
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            Debug.Log("Move Action Update!");
            return Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}