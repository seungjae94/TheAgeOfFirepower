using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public abstract class Page : Presenter
    {
        // History Management
        private class PageHistory
        {
            public Page page; 
            public List<PopupPresenter> popups = new();
        }
        
        private static readonly Stack<PageHistory> s_historyStack = new();
        public static Page CurrentPage { get; private set; }
        private static IMainCanvas CurrentCanvas => GameManager.Inst.CurrentCanvas; 
        
        // Field
        [SerializeField]
        protected AudioClip bgm;
        
        public void Open()
        {
            // 히스토리 백업
            if (CurrentPage != null)
            {
                CanvasLayer popupLayer = CurrentCanvas.GetLayer(ECanvasLayer.Popup);

                PageHistory history = new()
                {
                    page = CurrentPage,
                    popups = popupLayer.GetAllPresenters<PopupPresenter>()
                };
                s_historyStack.Push(history);
                
                // 페이지 콜백
                CurrentPage.OnClose();
            }

            // 열려있는 프레젠터 전부 비활성화
            // TODO: 불필요한 비활성화 예방 (다음 페이지로 이동하더라도 계속 열려 있어야 하는 경우)
            GameManager.Inst.CurrentCanvas.DeactivateAllPresenters();
            
            // 이 페이지 활성화
            Activate();
            CurrentPage = this;

            // 페이지 콜백
            OnOpen();
            
            // BGM 재생
            AudioManager.Inst.PlayBGM(bgm);
        }
        
        public void Close()
        {
            // 페이지 콜백
            OnClose();
            
            // 열려있는 프레젠터 전부 비활성화
            // TODO: 불필요한 비활성화 예방 (이전 페이지로 이동하더라도 계속 열려 있어야 하는 경우)
            GameManager.Inst.CurrentCanvas.DeactivateAllPresenters();
            
            if (s_historyStack.Count == 0)
                return;
            
            // 현재 페이지 변경
            PageHistory history = s_historyStack.Pop(); 
            CurrentPage = history.page;
            
            // 히스토리 복원
            CurrentPage.Activate();
            history.popups.ForEach(popup => popup.Activate());
            
            // 페이지 콜백
            CurrentPage.OnOpen();
            
            // BGM 재생
            AudioManager.Inst.PlayBGM(CurrentPage.bgm);
        }

        protected abstract void OnOpen();

        protected abstract void OnClose();
        
        // 페이지 이름
        public abstract string PageName { get; }
    }
}
