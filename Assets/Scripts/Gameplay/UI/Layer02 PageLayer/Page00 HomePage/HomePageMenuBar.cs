using System;
using Mathlife.ProjectL.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class HomePageMenuBar : AbstractView
    {
        private Button batteryMenuButton;
        private Button artilleryMenuButton;
        private Button inventoryMenuButton;
        private Button shopMenuButton;
        private Button battleMenuButton;

        private readonly CompositeDisposable disposables = new();

        public override void Initialize()
        {
            base.Initialize();
            
            batteryMenuButton = transform.FindRecursiveByName<Transform>("Battery Menu")
                .GetComponentInChildren<Button>();
            artilleryMenuButton = transform.FindRecursiveByName<Transform>("Artillery Menu")
                .GetComponentInChildren<Button>();
            inventoryMenuButton = transform.FindRecursiveByName<Transform>("Inventory Menu")
                .GetComponentInChildren<Button>();
            shopMenuButton = transform.FindRecursiveByName<Transform>("Shop Menu")
                .GetComponentInChildren<Button>();
            battleMenuButton = transform.FindRecursiveByName<Transform>("Battle Menu")
                .GetComponentInChildren<Button>();
        }

        public override void Draw()
        {
            disposables.Clear();
            
            batteryMenuButton.OnClickAsObservable()
                .Subscribe(_ => OpenPage(typeof(BatteryPage)))
                .AddTo(disposables);

            artilleryMenuButton.OnClickAsObservable()
                .Subscribe(_ => OpenPage(typeof(ArtyPage)))
                .AddTo(disposables);

            inventoryMenuButton.OnClickAsObservable()
                .Subscribe(_ => OpenPage(typeof(InventoryPage)))
                .AddTo(disposables);

            shopMenuButton.OnClickAsObservable()
                .Subscribe(_ => OpenPage(typeof(ShopPage)))
                .AddTo(disposables);

            battleMenuButton.OnClickAsObservable()
                .Subscribe(_ => OpenPage(typeof(StageSelectionPage)))
                .AddTo(disposables);
        }

        public override void Clear()
        {
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void OpenPage(Type type)
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            (Presenter.Find(type) as Page)?.Open();
        }
    }
}