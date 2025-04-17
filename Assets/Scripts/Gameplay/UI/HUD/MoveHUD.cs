using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class MoveHUD : Presenter, IInteractable
    {
        [SerializeField]
        private Button leftButton;
        
        [SerializeField]
        private Button rightButton;

        // Field
        private bool LBPressed = false;
        private bool RBPressed = false;
        private float horAxis = 0f;
        
        public override void Initialize()
        {
            base.Initialize();

            leftButton.OnPointerDownAsObservable()
                .Subscribe(_ => OnLBChanged(true))
                .AddTo(gameObject);
                
            leftButton.OnPointerUpAsObservable()
                .Subscribe(_ => OnLBChanged(false))
                .AddTo(gameObject);

            rightButton.OnPointerDownAsObservable()
                .Subscribe(_ => OnRBChanged(true))
                .AddTo(gameObject);
                
            rightButton.OnPointerUpAsObservable()
                .Subscribe(_ => OnRBChanged(false))
                .AddTo(gameObject);
            
            Observable.EveryUpdate()
                .Select(_ => Input.GetAxis("Horizontal"))
                .DistinctUntilChanged()
                .Subscribe(OnHorizontalAxisChanged)
                .AddTo(gameObject);
        }

        private void OnLBChanged(bool value)
        {
            if (value)
            {
                AudioManager.Inst.PlaySE(ESoundEffectId.Engine, true);
            }
            else
            {
                AudioManager.Inst.StopSE(true);
            }
            
            LBPressed = value;
            UpdateAxis();
        }
        
        private void OnRBChanged(bool value)
        {
            if (value)
            {
                AudioManager.Inst.PlaySE(ESoundEffectId.Engine, true);
            }
            else
            {
                AudioManager.Inst.StopSE(true);
            }
            
            RBPressed = value;
            UpdateAxis();
        }

        private void OnHorizontalAxisChanged(float value)
        {
            horAxis = value;
            UpdateAxis();
        }

        private void UpdateAxis()
        {
            var turnOwner = PlaySceneGameMode.Inst.turnOwner;
            
            if (turnOwner != null)
                turnOwner.MoveAxis = (LBPressed ? -1f : 0f) + (RBPressed ? 1f : 0f) + horAxis;
        }

        public void Enable()
        {
            leftButton.interactable = true;
            rightButton.interactable = true;
        }

        public void Disable()
        {
            leftButton.interactable = false;
            rightButton.interactable = false;
        }
    }
}