namespace Mathlife.ProjectL.Gameplay
{
    public static class Constants
    {
        public const int BATTERY_SIZE = 3;
        public const int RESOLUTION_HEIGHT = 1080;
    }

    public static class SceneNames
    {
        public const string APP_SCOPE_SCENE = "AppScopeScene";
        public const string TITLE_SCENE = "TitleScene";
        public const string LOBBY_SCENE = "LobbyScene";
        public const string PLAY_SCENE = "PlayScene";
    }

    public enum ETileCollisionType
    {
        Block,
    }

    public enum ETilemapType
    {
        Terrain = 0,
        Deco = 100,
        Collision = 200,
        Actor = 300,
    }
}