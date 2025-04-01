using Cysharp.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public abstract class TurnBase
    {
        // public static TurnBase Create()
        // {
        //     
        // }
        
        public abstract UniTask Logic();
        // public abstract UniTask<Decision> MakeDecision();
        // public abstract UniTask<TurnResult> EndTurn();
        //
        // public UniTask DispatchDecision(Decision decision)
        // {
        //     // TODO: decision 종류에 따라 적절한 decision 처리 함수 호출
        //     
        //     
        // }
        //
        // protected abstract UniTask UseItem(BattleItemDecision decision);
        // protected abstract UniTask Fire(FireDecision decision);
    }
}