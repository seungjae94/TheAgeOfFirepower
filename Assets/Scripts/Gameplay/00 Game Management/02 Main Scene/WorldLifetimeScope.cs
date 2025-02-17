using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mathlife.ProjectL.Gameplay
{
    public class WorldLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponentInHierarchy<WorldSceneManager>();
        }
    }
}
