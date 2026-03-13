using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private static int roundMax;
    private static int prizeStage = 0;
    private static float PrizeRate;
    private static int lendAmount = 0, lendTurn = 0, MaxLendTurn;
    public static PowerClass ActivePwr { get; private set; }
    private void Start() {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this; return;
    }
    public static void StartGame(int point, float prize_rate, int lendturn) {
        if (gstarted) return;
        gstarted = true;
        Point = point;
        PrizeRate = prize_rate;
        MaxLendTurn = lendturn;
        ActivePwr = PowerClass.Null;
        EdgeList.Init();
        PowerList.Init();
    }
    public static void EndGame() {
        if (!(gstarted & rstarted)) return;
        EndRound();
        gstarted = false; 
        EdgeList.Clear();
        PowerList.Clear();
        GameLeaderboard.UploadEntry(Point);
    }
    public static void StartRound(int min, int max) {
        if (!(gstarted & !rstarted)) return;
        rstarted = true;
        prizeStage = 0;
        roundMax = max;
        VertexCount = ValidVertexCount[
            UnityEngine.Random.Range(0, ValidVertexCount.Length - 1)
        ];
        Vertexes.Init(VertexCount, min, max);
        EdgePrizes.Init(VertexCount, PrizeRate);
        AddEdgeToList(RandomEdge(cost_mult: 0f), cost_mult: 0f);
    }
    private static void EndRound()  {
        if (!(gstarted & rstarted)) return;
        rstarted = false;
        Vertexes.Clear();
        EdgePrizes.Clear();
    }
    public static void ReplaceVertexValuesAtEdge(int idx, Tuple<int, int> edge) {
        if (edge == null) return;
        EdgeList.Remove(edge);
        int i = idx, j = (idx + VertexCount - 1) % VertexCount;
        ResolvePower(PowerClass.Pickup, Vertexes.At(i), Vertexes.At(j));
        int[] idxes = Vertexes.Edit(i, edge.Item1, j, edge.Item2);
        if (idxes.Length == 0) return;
        var mult = (prizeStage + 2f) / 2f;
        var sum = Mathf.FloorToInt(
            idxes.Sum(i => Vertexes.At(i)) * mult
        );
        Point += sum;
        EdgePrizes.AddPower(idxes);
        ResolvePower(PowerClass.Profit, sum);
        ResolvePower(PowerClass.Lend);
        EndRound();
    }
    public static void AddEdgeToList(Tuple<int, int> edge, float cost_mult = 1f) {
        var t_edge = RandomEdge(edge, cost_mult);
        if (t_edge == null) { Debug.LogWarning(""); return; }
        EdgeList.Add(t_edge);
        var cost = Mathf.CeilToInt(cost_mult * EdgeCost(t_edge));
        Point -= cost;
    }
    public static Tuple<int, int> RemoveEdgeFromListAt(int idx) {
        return EdgeList.RemoveAt(idx);
    }
    public static Tuple<int, int> GetEdgeFromListAt(int idx) {
        return EdgeList.Get(idx);
    }
    private static Tuple<int, int> RandomEdge(Tuple<int, int> edge = null, float cost_mult = 1f) {
        edge ??= new(0, 0);
        int l = edge.Item1, r = edge.Item2;
        var validpairs = new List<Tuple<int, int>>();
        for (int il = l == 0 ? 1 : l; il <= (l == 0 ? roundMax : l); ++il) {
            for (int ir = r == 0 ? 1 : r; ir <= (r == 0 ? roundMax : r); ++ir) {
                if (il == ir) continue;
                if ((il + ir) * cost_mult > Point) continue;
                validpairs.Add(new(il, ir));
            }
        }
        if (validpairs.Count == 0) return null;
        return validpairs[UnityEngine.Random.Range(0, validpairs.Count - 1)];
    }
    public static int EdgeCost(Tuple<int, int> p) {
        p ??= new(0, 0);
        return p.Item1 + p.Item2;
    }
    public static void ActivatePower(PowerClass pwr) {
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
        ActivePwr = pwr;

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
    public static void ResolvePower(PowerClass pwr, params int[] values)  {
        if (ActivePwr != pwr) return;
        // Debug.Log("Resolve!");
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
