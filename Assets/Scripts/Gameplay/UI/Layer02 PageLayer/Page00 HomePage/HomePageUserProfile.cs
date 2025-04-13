using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class HomePageUserProfile : AbstractView
    {
        [SerializeField]
        private TextMeshProUGUI userNameText;

        private readonly CompositeDisposable disposables = new();
        
        public override void Draw()
        {
            base.Draw();

            GameState.Inst.gameProgressState
                .userNameRx
                .DistinctUntilChanged()
                .Subscribe(userName => userNameText.text = userName)
                .AddTo(disposables);
        }

        public override void Clear()
        {
            base.Clear();
            
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}