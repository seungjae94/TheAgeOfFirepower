using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class WorldMapView : AbstractView
    {
        private StageInfoView[] stageInfoViews;
        
        public void Setup(int worldNo)
        {
            if (worldNo != 1)
            {
                throw new NotImplementedException("Not implemented for worldNo != 1");
            }

            var stageDataDict = GameState.Inst.gameDataLoader.GetWorldMapData(worldNo);
            stageInfoViews = GetComponentsInChildren<StageInfoView>();
            for (int i = 0; i < stageInfoViews.Length; i++)
            {
                stageInfoViews[i-1].Setup(stageDataDict[i]);
            }
        }

        public override void Draw()
        {
            base.Draw();
            
            foreach (var stageInfoView in stageInfoViews)
            {
                stageInfoView.Draw();
            }
        }

        public override void Clear()
        {
            base.Clear();
            
            foreach (var stageInfoView in stageInfoViews)
            {
                stageInfoView.Clear();
            }
        }
    }
}