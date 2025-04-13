using System.Collections.Generic;
using System.Linq;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class TrajectoryRenderer : MonoSingleton<TrajectoryRenderer>
    {
        // Field
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;

        private LineRenderer lineRenderer;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            lineRenderer = gameObject.GetComponent<LineRenderer>();
            Off();
        }

        public void Draw(List<Vector3> positions)
        {
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }

        public void On()
        {
            lineRenderer.enabled = true;
        }

        public void Off()
        {
            lineRenderer.enabled = false;
        }
    }
}