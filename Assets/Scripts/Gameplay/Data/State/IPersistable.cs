using Cysharp.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public interface IPersistable
    {
        UniTask Load();
        UniTask Save();
    }
}