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
    [NodeDescription(name: "Move", story: "[Agent] move according to [Strategy]", category: "Action",
        id: "4249e51cc56a5f54c3dbe97b9ab148e7")]
    public partial class MoveAction : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> Agent;

        [SerializeReference]
        public BlackboardVariable<MoveStrategy> Strategy;

        private ArtyController controller;
        private float axis = 0f;

        private float moveTime = 1.5f;
        private float moveTimer = 1.5f;
        
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

            float averageX = PlaySceneGameMode.Inst.AlivePlayers.Average(player => player.transform.position.x);

            // 플레이어가 더 왼쪽에 있는 경우
            if (averageX < Agent.Value.transform.position.x)
            {
                if (Strategy.Value == MoveStrategy.InFighter)
                {
                    axis = -1f;
                }
                else if (Strategy.Value == MoveStrategy.OutBoxer)
                {
                    axis = 1f;
                }                
            }
            else
            {
                if (Strategy.Value == MoveStrategy.InFighter)
                {
                    axis = 1f;
                }
                else if (Strategy.Value == MoveStrategy.OutBoxer)
                {
                    axis = -1f;
                }
            }
            
            
            controller = Agent.Value.GetComponent<ArtyController>();
            moveTimer = moveTime;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            controller.MoveAxis = axis;            
            moveTimer -= Time.deltaTime;
            
            if (controller.CurrentFuel <= 0f || moveTimer <= 0f)
            {
                controller.MoveAxis = 0f;
                return Status.Success;
            }
            
            return Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}