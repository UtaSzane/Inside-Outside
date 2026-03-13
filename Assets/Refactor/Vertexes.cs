using System;
using System.Collections.Generic;

#pragma warning disable IDE0130
namespace Test {
#pragma warning restore IDE0130

    public static class Vertexes {
        private static int[] values;

        // public static int[] Values()
        // {
        //     return values;
        // }
        public static void Init(int len, int min, int max)
        {
            values = new int[len];
            for (int i = 0; i < len; ++i) {
                int j1 = (i + 1) % len, j2 = (i + len - 1) % len;
                do {
                    values[i] = UnityEngine.Random.Range(min, max + 1);
                }
                while (
                    values[i] == values[j1] || values[i] == values[j2]
                );
            }
        }
        public static void Clear()
        {
            values = null;
        }
        public static int[] Edit(int idx1, int val1, int idx2, int val2)
        {
            values[idx1] = val1; values[idx2] = val2;

            var indexes = new List<int>();
            var cnt = GameLogic.VertexCount;
            for (int i = 0; i < cnt; ++i)
            {
                if (values[i] == values[(i + cnt - 1) % cnt])
                {
                    indexes.Add(i);
                }
            }
            return indexes.ToArray();
        }
        public static int At(int idx)
        {
            return values[idx];
        }
    }
}