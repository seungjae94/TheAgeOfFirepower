namespace Mathlife.ProjectL.Gameplay
{
    public class WorldMenuBar : Presenter
    {
        public void OnClickTeamManagementButton()
        {
            m_worldSceneManager.Navigate(EPageId.TeamPage);
        }

        public void OnClickInventoryButton()
        {
            m_worldSceneManager.Navigate(EPageId.InventoryPage);
        }
    }
}
