namespace Mathlife.ProjectL.Utils
{
    public class MyMathf
    {
        /// <summary>
        /// int/int는 rounds towards zero 방식으로 동작한다.
        /// 어느 한 쪽이 음수일 경우 FloorDiv를 사용해야 내림 방식이 적용된다.
        /// </summary>
        public static int FloorDiv(int a, int b)
        {
            if (((a < 0) ^ (b < 0)) && (a % b != 0))
            {
                return (a/b - 1);
            }
            else
            {
                return (a/b);
            }
        }
    }
}