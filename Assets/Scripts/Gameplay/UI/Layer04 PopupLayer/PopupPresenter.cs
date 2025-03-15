using Cysharp.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public abstract class PopupPresenter : Presenter
    {
        
        public virtual UniTask OpenWithAnimation()
        {
            base.Activate();
            return UniTask.CompletedTask;
        }
        
        public virtual UniTask CloseWithAnimation()
        {
            base.Deactivate();
            return UniTask.CompletedTask;
        }
    }
}