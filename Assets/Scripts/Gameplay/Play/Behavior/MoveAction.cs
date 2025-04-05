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

        private ArtyController controller;
        private float axis = 0f;
        private float timer = 0f;
        private float time = 0f;
        
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
            
            Debug.Log($"Move Strategy: {Strategy.Value}");
            
            // TODO: 목적지 계산
            time = 0.75f;
            timer = 0f;
            axis = -1f;
            controller = Agent.Value.GetComponent<ArtyController>();
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            timer += Time.deltaTime;
            controller.MoveAxis = axis;            
            
            if (timer >= time)
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