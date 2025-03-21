namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryMechPartTabView : AbstractView
    {
        // View
        // TODO: InventoryMechPartScrollView
        // TODO: InventorySelectedMechPartView
        
        // Field
        // TODO: ReactiveProperty<int> selectedIndexRx
        
        public override void Draw()
        {
            base.Draw();
            gameObject.SetActive(true);

        }
        
        public override void Clear()
        {
            base.Clear();
            gameObject.SetActive(false);

        }
    }
}