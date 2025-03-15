using Cysharp.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public abstract class PopupPresenter : Presenter
    {
        
        public virtual UniTask OpenWithAnimation()
        {
            base.Open();
            return UniTask.CompletedTask;
        }
        
        public virtual UniTask CloseWithAnimation()
        {
            base.Close();
            return UniTask.CompletedTask;
        }
    }
}