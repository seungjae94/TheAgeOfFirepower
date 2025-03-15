using System.Collections.Generic;
using System.Linq;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public abstract class Page : Presenter
    {
        // History Management
        private class PageHistory
        {
            public Page page; 
            public List<OverlayPresenter> overlays = new();
            public List<PopupPresenter> popups = new();
        }
        
        private static readonly Stack<PageHistory> s_historyStack = new();
        public static Page CurrentPage { get; private set; }

        private static IMainCanvas CurrentCanvas => GameManager.Inst.CurrentCanvas; 
 
        public virtual void Open()
        {
            // 히스토리 백업
            if (CurrentPage != null)
            {
                CanvasLayer overlayLayer = CurrentCanvas.GetLayer(ECanvasLayer.Overlay);
                CanvasLayer popupLayer = CurrentCanvas.GetLayer(ECanvasLayer.Popup);

                PageHistory history = new()
                {
                    page = CurrentPage,
                    overlays = overlayLayer.GetAllPresenters<OverlayPresenter>(),
                    popups = popupLayer.GetAllPresenters<PopupPresenter>()
                };
                s_historyStack.Push(history);
            }

            // 열려있는 프레젠터 전부 비활성화
            GameManager.Inst.CurrentCanvas.DeactivateAllPresenters();
            
            // 이 페이지 활성화
            Activate();
            CurrentPage = this;
        }

        public virtual void Close()
        {
            // 열려있는 프레젠터 전부 비활성화
            GameManager.Inst.CurrentCanvas.DeactivateAllPresenters();
            
            if (s_historyStack.Count == 0)
                return;
            
            // 히스토리 복원
            PageHistory history = s_historyStack.Pop();
            history.page.Activate();
            history.overlays.ForEach(overlay => overlay.Activate());
            history.popups.ForEach(popup => popup.Activate());
            
            // 현재 페이지 변경
            CurrentPage = history.page;
        }
        
        // 페이지 이름
        public abstract string PageName { get; }
    }
}
