using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mathlife.ProjectL.Gameplay
{
    public class MainSceneLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponentInHierarchy<MainSceneManager>();

            builder.Register<CharacterSelectionFlexItemFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ShopFlexItemFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<InventorySlotFlexItemFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}
