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
        public float Axis 
            => Mathf.Clamp((LBPressed ? -1f : 0f) + (RBPressed ? 1f : 0f) + horAxis, -1f, 1f);

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
                .Subscribe(value => horAxis = value)
                .AddTo(gameObject);
        }

        private void OnLBChanged(bool value)
        {
            LBPressed = value;
        }
        
        private void OnRBChanged(bool value)
        {
            RBPressed = value;
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