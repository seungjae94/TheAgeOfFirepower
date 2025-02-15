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
        public GameDataDB gameDataDB;
        public SaveDataDB saveDataDB;
        public CharacterRepository characterRepository;
        public InventoryRepository inventoryRepository;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(gameDataDB);
            builder.RegisterInstance(saveDataDB);
            builder.RegisterInstance(characterRepository);
            builder.RegisterInstance(inventoryRepository);
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
