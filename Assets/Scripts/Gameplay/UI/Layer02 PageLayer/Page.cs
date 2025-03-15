using System.Collections.Generic;
using System.Linq;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public abstract class Page : Presenter
    {
        private class PageHistory
        {
            public Page page; 
            public List<OverlayPresenter> overlays = new();
            public List<PopupPresenter> popups = new();
        }
        
        private static readonly Stack<PageHistory> s_historyStack = new();
        public static Page CurrentPage => s_historyStack.LastOrDefault()?.page;

        private static IMainCanvas CurrentCanvas => GameManager.Inst.CurrentCanvas; 
        
        public static void NavigateBack()
        {
            CurrentPage.Close();
        }
        
        public override void Open()
        {
            base.Open();
            
            if (s_historyStack.Count == 0)
                return;
            
            CanvasLayer overlayLayer = CurrentCanvas.GetLayer(ECanvasLayer.Overlay);
            CanvasLayer popupLayer = CurrentCanvas.GetLayer(ECanvasLayer.Popup);
            
            PageHistory history = new()
            {
                page = CurrentPage,
                overlays = overlayLayer.GetAllPresenters<OverlayPresenter>(),
                popups = popupLayer.GetAllPresenters<PopupPresenter>()
            };
            s_historyStack.Push(history);
            
            // 이전 페이지 닫기
            history.page.Close();
            history.overlays.ForEach(overlay => overlay.Close());
            history.popups.ForEach(popup => popup.Close());
        }

        public override void Close()
        {
            base.Close();

            if (s_historyStack.Count == 0)
                return;
            
            // 이전 페이지 열기
            PageHistory history = s_historyStack.Pop();
            history.page.Open();
            history.overlays.ForEach(overlay => overlay.Open());
            history.popups.ForEach(popup => popup.Open());
        }
    }
}
