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
            builder.RegisterComponentInHierarchy<HomePage>();
            builder.RegisterComponentInHierarchy<PartyPage>();
            builder.RegisterComponentInHierarchy<CharacterDetailPage>();
            builder.RegisterComponentInHierarchy<InventoryPage>();
            builder.RegisterComponentInHierarchy<ShopPage>();

            builder.Register<CharacterSelectionFlexItemFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ShopFlexItemFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<InventoryFlexItemFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

        }
    }
}
