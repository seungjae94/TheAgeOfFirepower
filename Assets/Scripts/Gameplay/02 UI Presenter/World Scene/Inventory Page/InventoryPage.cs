namespace Mathlife.ProjectL.Gameplay
{
    public class InventoryPage : Page
    {
        public override EPageId pageId => EPageId.InventoryPage;
        public override void Initialize()
        {
            InitializeChildren();

            Close();
        }

        protected override void InitializeChildren()
        {
        }
    }
}