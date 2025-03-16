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
            batteryMenuButton.OnClickAsObservable()
                .Subscribe(_ => Presenter.Find<BatteryPage>().Open())
                .AddTo(disposables);

            artilleryMenuButton.OnClickAsObservable()
                .Subscribe(_ => Presenter.Find<ArtySelectionPage>().Open())
                .AddTo(disposables);

            inventoryMenuButton.OnClickAsObservable()
                .Subscribe(_ => Presenter.Find<InventoryPage>().Open())
                .AddTo(disposables);

            shopMenuButton.OnClickAsObservable()
                .Subscribe(_ => Presenter.Find<ShopPage>().Open())
                .AddTo(disposables);

            battleMenuButton.OnClickAsObservable()
                .Subscribe(_ => Presenter.Find<StageSelectionPage>().Open())
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
    }
}