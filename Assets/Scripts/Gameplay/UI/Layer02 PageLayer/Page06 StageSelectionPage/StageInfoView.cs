using System;
using Mathlife.ProjectL.Gameplay.Stage;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageInfoView : AbstractView
    {

        // Field
        private StageGameData stageGameData;
        private readonly CompositeDisposable disposables = new();
        
        public void Setup(StageGameData pStageGameData)
        {
            stageGameData = pStageGameData;
        }

        public override void Draw()
        {
            base.Draw();
            
            // TODO: Locked 여부 및 선택 여부에 따라 다르게 렌더링
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