using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable IDE0130
namespace Test {
#pragma warning restore IDE0130

    public enum PowerClass {
        Pickup, Profit, Consume, Upgrade, Lend, 
        SIZE, Null
    }

    [DisallowMultipleComponent]
    public class GameLogic: MonoBehaviour {
        private static readonly int[] ValidVertexCount = new int[] {
            3, 4, 5, 6, 8, 9, 10, 12, 15, 18, 20
        };
        private static GameLogic instance;
        public static int VertexCount { get; private set; }
        public static int Point { get; private set; }
        private static bool rstarted = false, gstarted = false;
        public static bool GameStarted => gstarted;
        public static bool RoundStarted => rstarted;
        private static int roundMax, roundMin;
        private static int prizeStage = 0;
        private static int lendAmount = 0, lendTurn = 0, MaxLendTurn;
        public static PowerClass ActivePwr { get; private set; }
        public static List<int> Indexes { get; private set; }
        private void Start() {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this; return;
        }
        public static void StartGame()
        {
            if (gstarted) return;
            gstarted = true;
            EdgeList.Init();
            PowerList.Init();
            Indexes.Clear();
        }
        public static void EndGame() 
        {
            if (!(gstarted & rstarted)) return;
            EndRound();
            gstarted = false; 
            EdgeList.Clear();
            PowerList.Clear();
        }
        public static void StartRound(int min, int max, float prize_rate, int point, int lendturn)
        {
            if (!(gstarted & !rstarted)) return;
            rstarted = true;
            Point = point;
            prizeStage = 0;
            roundMax = max;
            roundMin = min;
            MaxLendTurn = lendturn;
            VertexCount = ValidVertexCount[
                UnityEngine.Random.Range(0, ValidVertexCount.Length - 1)
            ];
            for (int i = 0; i < VertexCount; ++i) Indexes.Add(i);
            Vertexes.Init(VertexCount, min, max);
            EdgePrizes.Init(VertexCount, prize_rate);
            AddEdgeToList(RandomEdge(), cost_mult: 0f);
        }
        private static void EndRound() 
        {
            if (!(gstarted & rstarted)) return;
            rstarted = false;
            Vertexes.Clear();
            EdgePrizes.Clear();
        }
        public static void ReplaceVertexValuesAtEdge(int idx, Tuple<int, int> edge) 
        {
            EdgeList.Remove(edge);
            int i = idx, j = (idx + VertexCount - 1) % VertexCount;
            ResolvePower(PowerClass.Pickup, Vertexes.At(i), Vertexes.At(j));
            int[] idxes = Vertexes.Edit(
                i, edge.Item1,
                j, edge.Item2
            );
            if (idxes.Length == 0) return;
            var mult = (prizeStage + 2f) / 2f;
            var sum = Mathf.FloorToInt(
                idxes.Sum(i => Vertexes.At(i)) * mult
            );
            Point += sum;
            EdgePrizes.AddPower(idxes);
            ResolvePower(PowerClass.Profit, sum);
            EndRound();
            ResolvePower(PowerClass.Lend);
        }
        public static void AddEdgeToList(Tuple<int, int> edge, float cost_mult = 1f) 
        {
            EdgeList.Add(RandomEdge(edge, cost_mult));
            Point -= Mathf.CeilToInt(cost_mult * EdgeCost(edge));
        }
        public static Tuple<int, int> RemoveEdgeFromListAt(int idx) 
        {
            return EdgeList.RemoveAt(idx);
        }
        public static Tuple<int, int> GetEdgeFromListAt(int idx) 
        {
            return EdgeList.Get(idx);
        }
        private static Tuple<int, int> RandomEdge(Tuple<int, int> edge = null, float cost_mult = 1f) 
        {
            edge ??= new(0, 0);
            int l = edge.Item1, r = edge.Item2;
            if (cost_mult == 0f)
            {
                do
                {
                    l = l != 0 ? l : UnityEngine.Random.Range(1, roundMax + 1);
                    r = r != 0 ? r : UnityEngine.Random.Range(1, roundMax + 1); 
                } while (l == r);
                return new(l, r);
            }
            if (l == 0 && r == 0)
            {
                do
                {
                    int max_l = (int)Mathf.Min(roundMax, Point / cost_mult);
                    l = UnityEngine.Random.Range(1, max_l + 1);

                    int max_r = (int)Mathf.Min(roundMax, Point / cost_mult - l);
                    if (max_r <= 0) r = l;
                    else r = UnityEngine.Random.Range(1, max_r + 1); 
                } while (l == r);
                return new(l, r);
            }
            if (l == 0 || r == 0)
            {
                int lrnot0 = l == 0 ? r : l;
                int maxother = (int)Mathf.Min(roundMax, Point / cost_mult - lrnot0);
                if (maxother <= 0) return null;

                int other = UnityEngine.Random.Range(1, maxother + 1); 
                return new(l == 0 ? other : l, r == 0 ? other : r);
            }
            return (l == r || (l + r) * cost_mult > Point) ? null : new(l, r);
        }
        public static int EdgeCost(Tuple<int, int> p) 
        {
            return p.Item1 + p.Item2;
        }
        public static void ActivatePower(PowerClass pwr) 
        {
            if (ActivePwr != PowerClass.Null) return;
            
            switch (pwr) {
                case PowerClass.Pickup: break;
                case PowerClass.Profit: break;
                case PowerClass.Upgrade: break;
                case PowerClass.Consume: {
                    if (EdgeList.Count == 0) return;
                    break;
                }
                case PowerClass.Lend: {
                    if (Point == 0) return;
                    break;
                }
            }

            if (!PowerList.RemovePower(pwr)) return;

            switch (pwr) {
                case PowerClass.Pickup: break;
                case PowerClass.Profit: break;
                case PowerClass.Upgrade: break;
                case PowerClass.Consume: break;
                case PowerClass.Lend: {
                    lendTurn = MaxLendTurn;
                    lendAmount = Mathf.FloorToInt(Point / 2f);
                    Point -= lendAmount;
                    break;
                }
            }
        }
        public static void ResolvePower(PowerClass pwr, params int[] values) 
        {
            if (ActivePwr != pwr) return;

            switch (pwr) {
                case PowerClass.Pickup: {
                    EdgeList.Add(new(values[0], values[1]));
                    goto default;
                }
                case PowerClass.Profit: {
                    Point += values[0];
                    goto default;
                }
                case PowerClass.Upgrade: {
                    prizeStage += 1;
                    goto default;
                }
                case PowerClass.Consume: {
                    EdgeList.Remove(new(values[0], values[1]));
                    Point += values[0] + values[1];
                    goto default;
                }
                case PowerClass.Lend: {
                    lendTurn -= 1;
                    if (lendTurn > 0) break;
                    Point += 2 * lendAmount;
                    lendAmount = 0;
                    goto default;
                }
                default: {
                    ActivePwr = PowerClass.Null;
                    break;
                }
            }
        }
    }
}