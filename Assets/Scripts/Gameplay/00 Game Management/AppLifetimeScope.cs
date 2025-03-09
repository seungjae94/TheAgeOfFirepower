using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mathlife.ProjectL.Gameplay
{
    public class AppLifetimeScope : LifetimeScope
    {
        public GameDataLoader gameDataLoader;
        public SaveDataManager saveDataManager;
        public CharacterRosterState CharacterRosterState;
        public InventoryState InventoryState;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(gameDataLoader);
            builder.RegisterInstance(saveDataManager);
            builder.RegisterInstance(CharacterRosterState);
            builder.RegisterInstance(InventoryState);
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
