using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Mathlife.ProjectL.Gameplay.Gameplay.Data.Model
{
    public class GameProgressState : PersistableStateBase
    {
        private static readonly Regex regexCharset = new Regex(@"^[a-zA-Z0-9가-힣]+$", RegexOptions.Compiled);
        private static readonly Regex regexReplace = new Regex(@"[^a-zA-Z0-9가-힣]", RegexOptions.Compiled);
        
        // Props
        public readonly ReactiveProperty<int> unlockWorldRx = new(1);
        public readonly ReactiveProperty<int> unlockStageRx = new(1);
        public readonly ReactiveProperty<string> userNameRx = new("");
        
        public override UniTask Load()
        {
            if (GameState.Inst.saveDataManager.CanLoad() && DebugSettings.Inst.UseSaveFileIfAvailable)
            {
                var saveFile = SaveDataManager.GameProgress;
                unlockWorldRx.Value = saveFile.unlockWorld;
                unlockStageRx.Value = saveFile.unlockStage;
                userNameRx.Value = saveFile.userName;
                return UniTask.CompletedTask;
            }

            var starterData = GameDataLoader.GetStarterData();
            unlockWorldRx.Value = starterData.GetStarterUnlockWorldNo();
            unlockStageRx.Value = starterData.GetStarterUnlockStageNo();
            userNameRx.Value = "테스트계정";
            return UniTask.CompletedTask;
        }

        protected override SaveFile SavedFile => SaveDataManager.GameProgress;
        protected override SaveFile TakeSnapShot()
        {
            return new GameProgressSaveFile
            {
                unlockWorld = unlockWorldRx.Value,
                unlockStage = unlockStageRx.Value,
                userName = userNameRx.Value
            };
        }

        
        public bool IsCurrentUserNameValid()
        {
            return IsUserNameValid(userNameRx.Value);
        }

        // 한글 2 ~ 6 글자, 영문 및 숫자 4 ~ 12 글자
        // 공백 및 특수문자는 허용하지 않음
        public static bool IsUserNameValid(string userName)
        {
            if (false == regexCharset.IsMatch(userName))
                return false;

            // (실제 바이트 수와 무관하게) 한글 2 바이트, 영문 및 숫자 1 바이트로 계산.
            // 합이 4 이상 12 이하여야 한다.
            int totalByte = userName.Aggregate<char, int>(0, (acc, cur) => acc + GetCallCharSize(cur));
            return totalByte is >= 4 and <= 12;
        }
        
        public static bool TryMakeValidUserName(string userName, out string newUserName)
        {
            userName = regexReplace.Replace(userName, "");

            if (userName.Any() == false)
            {
                newUserName = userName;
                return false;
            }
            
            IList<int> cumulativeSum = userName.Select((c, i) => GetCallCharSize(c)).ToList();
            for (int i = 1; i < cumulativeSum.Count(); i++)
            {
                cumulativeSum[i] += cumulativeSum[i - 1];

                // 12 글자를 초과할 경우 뒷 부분은 잘라내고 반환 
                if (cumulativeSum[i] > 12)
                {
                    newUserName = userName.Substring(0, i);;
                    return true;
                }
            }

            newUserName = userName;
            return cumulativeSum[^1] >= 4;
        }

        private static int GetCallCharSize(char c)
        {
            return c is >= '가' and <= '힣' ? 2 : 1;
        }
    }
}