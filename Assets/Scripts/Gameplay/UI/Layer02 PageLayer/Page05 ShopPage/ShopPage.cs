using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ShopPage : Page
    {
        public override string PageName => "상점";

        // View
        [SerializeField]
        private ShopTabMenuBar tabMenuBar;
        
        [SerializeField]
        private ShopArtyScrollRect artyTabView;
        
        [SerializeField]
        private ShopItemScrollRect itemTabView;
        
        // Field
        public readonly ReactiveProperty<int> selectedTabIndexRx = new(1);
        private readonly CompositeDisposable disposables = new();

        private ShopGameData shopGameData;
        
        public override void Initialize()
        {
            base.Initialize();

            shopGameData = GameState.Inst.gameDataLoader.GetShopData();
        }
        
        protected override void OnOpen()
        {
            // Overlay
            Find<NavigateBackOverlay>().Activate();
            Find<CurrencyBar>().Activate();
            
            // 뷰 초기화
            artyTabView.gameObject.SetActive(false);
            itemTabView.gameObject.SetActive(false);
            
            selectedTabIndexRx.Value = 0;
            selectedTabIndexRx
                .DistinctUntilChanged()
                .Subscribe(OnSelectTab)
                .AddTo(disposables);
            
            var tabMenuItemDataList = new List<ShopTabMenuItemData>()
            {
                new ShopTabMenuItemData() { displayName = "화포" },
                new ShopTabMenuItemData() { displayName = "포신" },
                new ShopTabMenuItemData() { displayName = "장갑" },
                new ShopTabMenuItemData() { displayName = "엔진" },
                new ShopTabMenuItemData() { displayName = "재료" },
                new ShopTabMenuItemData() { displayName = "전투" },
            };
            
            tabMenuBar.UpdateContents(tabMenuItemDataList);
            tabMenuBar.SelectCell(selectedTabIndexRx.Value);
        }

        protected override void OnClose()
        {
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        // 이벤트 구독 콜백
        private void OnSelectTab(int index)
        {
            switch (index)
            {
                case 0:
                    itemTabView.gameObject.SetActive(false);
                    artyTabView.gameObject.SetActive(true);
                    artyTabView.UpdateContents(shopGameData.shopArtyList);
                    break;
                case 1:
                    itemTabView.gameObject.SetActive(true);
                    artyTabView.gameObject.SetActive(false);
                    itemTabView.UpdateContents(shopGameData.shopBarrels);
                    break;
                case 2:
                    itemTabView.gameObject.SetActive(true);
                    artyTabView.gameObject.SetActive(false);
                    itemTabView.UpdateContents(shopGameData.shopArmors);
                    break;
                case 3:
                    itemTabView.gameObject.SetActive(true);
                    artyTabView.gameObject.SetActive(false);
                    itemTabView.UpdateContents(shopGameData.shopEngines);
                    break;
                case 4:
                    itemTabView.gameObject.SetActive(true);
                    artyTabView.gameObject.SetActive(false);
                    itemTabView.UpdateContents(shopGameData.shopMaterialItems);
                    break;
                case 5:
                    itemTabView.gameObject.SetActive(true);
                    artyTabView.gameObject.SetActive(false);
                    itemTabView.UpdateContents(shopGameData.shopBattleItems);
                    break;
            }
        }
    }
}
