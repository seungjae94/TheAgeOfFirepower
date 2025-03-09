using Cysharp.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public interface IState
    {
        UniTask Load();
        UniTask Save();
    }
}