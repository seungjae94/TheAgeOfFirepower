namespace Mathlife.ProjectL.Gameplay
{
    public static class Constants
    {
        public const int TeamMaxCount = 3;
        public const int PartyMemberCount = 3;
    }

    public static class SceneNames
    {
        public const string AppScopeScene = "AppScopeScene";
        public const string TitleScene = "TitleScene";
        public const string LobbyScene = "LobbyScene";
        public const string PlayScene = "PlayScene";
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