using Mathlife.ProjectL.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class HomePage : Page
    {
        private Button batteryMenuButton;
        private Button artilleryMenuButton;
        private Button inventoryMenuButton;
        private Button shopMenuButton;
        private Button battleMenuButton;

        public override void Open()
        {
            base.Open();

            // Page Overlay
            // TODO: UserInfo 구현 및 Open
            Find<CurrencyBar>().Open();
        }

        private void Awake()
        {
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
        
        private void Start()
        {
            batteryMenuButton.OnClickAsObservable()
                .Subscribe(_ => Find<BatteryPage>().Open())
                .AddTo(gameObject);

            artilleryMenuButton.OnClickAsObservable()
                .Subscribe(_ => Find<ArtySelectionPage>().Open())
                .AddTo(gameObject);

            inventoryMenuButton.OnClickAsObservable()
                .Subscribe(_ => Find<InventoryPage>().Open())
                .AddTo(gameObject);

            shopMenuButton.OnClickAsObservable()
                .Subscribe(_ => Find<ShopPage>().Open())
                .AddTo(gameObject);

            battleMenuButton.OnClickAsObservable()
                .Subscribe(_ => Find<StageSelectionPage>().Open())
                .AddTo(gameObject);
        }
    }
}