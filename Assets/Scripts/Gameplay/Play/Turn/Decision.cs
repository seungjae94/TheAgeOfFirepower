namespace Mathlife.ProjectL.Gameplay.Play
{
    public enum EDecisionType
    {
        Skip,
        Move,
        UseItem,
        Fire
    }
    
    public abstract class Decision
    {
        public EDecisionType Type { get; private set; }
     
        public Decision(EDecisionType type)
        {
            Type = type;
        }
    }
}