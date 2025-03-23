using System;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class ItemStackModel
    {
        public ItemStackModel(CountableItemGameData gameData, int amount)
        {
            this.gameData = gameData;
            this.Amount = amount;
        }
        
        private readonly CountableItemGameData gameData;
        
        public int Amount { get; private set; }

        public void Add(int value)
        {
            if (value <= 0)
                throw new ArgumentException("[ItemStackModel] param value can't be non-positive.");

            Amount = checked(Amount + value);
        }
        
        public void Remove(int value)
        {
            if (value <= 0)
                throw new ArgumentException("[ItemStackModel] param value can't be non-positive.");
            
            if (value > Amount)
                throw new ArgumentException("[ItemStackModel] param value can't be greater than the amount.");
            
            Amount -= value;
        }
        
        
        public int Id => gameData.id;
        
        public EItemRarity Rarity => gameData.rarity;

        public string DisplayName => gameData.displayName;
        
        public string Description => gameData.description;
        
        public Sprite Icon => gameData.icon;
    }
}