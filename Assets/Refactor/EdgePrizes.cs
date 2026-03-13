using System.Linq;
using UnityEngine;

#pragma warning disable IDE0130
namespace Test {
#pragma warning restore IDE0130

    public static class EdgePrizes {
        private static PowerClass[] prizes;

        private static PowerClass Random() {
            int idx = UnityEngine.Random.Range(0, (int)PowerClass.SIZE);
            return (PowerClass)idx;
        }
        public static void Init(int len, float rate)
        {
            prizes = new PowerClass[len];
            float prize_cnt = Mathf.Round(len * rate);
            for (int idx = 0; idx < len; ++idx) {
                var mark = 1f - (float)prize_cnt / (len - idx);
                var flag = UnityEngine.Random.value;
                if (flag >= mark) {
                    prizes[idx] = Random();
                    prize_cnt -= 1f;
                }
                else prizes[idx] = PowerClass.Null;
            }
        }
        public static void Clear()
        {
            prizes = null;
        }
        public static void AddPower(params int[] indexes)
        {
            foreach (int idx in indexes.Where(idx => Prized(idx)))
            {
                PowerList.AddPower(prizes[idx]);
            }
        }
        public static bool Prized(int index)
        {
            return prizes[index] != PowerClass.Null;
        }
    }
}