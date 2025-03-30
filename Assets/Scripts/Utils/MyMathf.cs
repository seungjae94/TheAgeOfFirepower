using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Utils
{
    public static class MyMathf
    {
        /// <summary>
        /// int/int는 rounds towards zero 방식으로 동작한다.
        /// 어느 한 쪽이 음수일 경우 FloorDiv를 사용해야 내림 방식이 적용된다.
        /// </summary>
        public static int FloorDiv(int a, int b)
        {
            if (((a < 0) ^ (b < 0)) && (a % b != 0))
            {
                return (a / b - 1);
            }
            else
            {
                return (a / b);
            }
        }

        public static int Mod(int x, int m)
        {
            int r = x % m;
            return (r < 0) ? r + m : r;
        }
        
        // 12시 방향부터 시계 방향
        private static readonly int[] dx = new int[8] { 0, 1, 1, 1, 0, -1, -1, -1 };
        private static readonly int[] dy = new int[8] { 1, 1, 0, -1, -1, -1, 0, 1 };
        private static readonly int[] enterDirsCW =  new int[8] { 2, 2, 4, 4, 6, 6, 0, 0 };
        private static readonly int[] enterDirsCCW =  new int[8] { 6, 0, 0, 2, 2, 4, 4, 6 };
        
        public static List<Vector2Int> MooreNeighbor(Vector2Int start, int count, Func<int, int, bool> gridReader)
        {
            // 파라미터 수정
            bool clockWise = count > 0;
            count = Mathf.Abs(count);

            // 진입 방향 찾기
            int enterDir = -1;
            for (int d = 0; d < 8; d++)
            {
                bool value = gridReader(start.x + dx[d], start.y + dy[d]);

                if (value == false)
                {
                    enterDir = (d + 4) % 8;
                    break;
                }
            }

            if (enterDir < 0)
                throw new Exception("표면 위치가 아니기 때문에 진입 방향을 찾지 못했습니다.");
            
            // 컨투어 계산 시작
            List<Vector2Int> boundary = new() { start };
            Vector2Int current = start;

            for (int i = 0; i < count; ++i)
            {
                int backtrackDir = (enterDir + 4) % 8;

                for (int d = 0; d < 8; ++d)
                {
                    int testDir = clockWise ? MyMathf.Mod(backtrackDir + d, 8) : MyMathf.Mod(backtrackDir - d, 8);

                    Vector2Int next = new Vector2Int(current.x + dx[testDir], current.y + dy[testDir]);
                    bool value = gridReader(next.x, next.y);

                    if (value)
                    {
                        enterDir = clockWise ? enterDirsCW[testDir] :  enterDirsCCW[testDir];
                        current = next;
                        boundary.Add(current);
                        break;
                    }
                }
            }

            return boundary;
        }
    }
}