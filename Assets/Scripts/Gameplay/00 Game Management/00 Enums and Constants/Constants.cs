namespace Mathlife.ProjectL.Gameplay
{
    public static class Constants
    {
        public const int TeamMaxCount = 3;
        public const int TeamMemberMaxCount = 3;
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