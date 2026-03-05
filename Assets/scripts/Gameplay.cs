using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Ability {
    Pickup, Profit, Dismantle,
    SIZE, Null
}

[DisallowMultipleComponent]
public class Gameplay: MonoBehaviour {
    private static Gameplay instance;
    [SerializeField] [Min(0)] private int Starter_Point = 100;
    [SerializeField] private List<Stats.Stages> stages;
    private void Start() {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this; return;
    }
    public static void GameStart() {
        if (instance == null) return;
        Stats.Init(instance.stages, instance.Starter_Point);
        Vertexes.AdvInit();
        PrizedEdges.AdvInit();
        Edges.Clear();
        Stats.GameStart = true;
        Edges.AddRandomEdge(cost: false);

        GameInterface.UpdateAbilityList();
        GameDisplay.MapCreate();
    }
    public void Update() {
        if (instance == null) return;
        if (!Stats.GameStart) return;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) Control.Left();
            else Control.Right();        
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
            if (Input.GetKeyDown(KeyCode.UpArrow)) Control.Up();
            else Control.Down();        
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            Control.Space();
        }
    }
    public static void GameEnd() {
        if (instance == null) return;
        Stats.GameStart = false;
        Vertexes.Clear();
        PrizedEdges.Clear();
        Edges.Clear();
    }
}


public static class Abilities {
    private readonly static HashSet<Ability> abilities = new();
    public static Ability[] AbilityList => abilities.ToArray();
    public static Ability active = Ability.Null;
    public static void Clear() {
        abilities.Clear();
        active = Ability.Null;
    }
    public static void AddNewAbility(Ability ability) {
        if (ability == Ability.Null || ability == Ability.SIZE) return;
        if (abilities.Contains(ability)) return;
        abilities.Add(ability);
        GameInterface.UpdateAbilityList();
    }
    public static void Activate(Ability ability) {
        if (active != Ability.Null) return;
        if (ability == Ability.Null || ability == Ability.SIZE) return; 
        if (!abilities.Contains(ability)) return; 
        abilities.Remove(ability);
        active = ability;
        GameInterface.UpdateAbilityList();
    }
    public static void Resolve() {
        if (active == Ability.Null) return;
        active = Ability.Null;
    }
}

public static class Vertexes {
    private static readonly int[] valid_orbcnt = new int[] {3, 4, 5, 6, 8, 9, 10, 12, 15};
    private static int[] values;
    public static int Count => values.Length;
    public static int Vertex(int idx) => values[idx];
    public static void AdvInit() {
        int length = valid_orbcnt[UnityEngine.Random.Range(0, valid_orbcnt.Length - 1)];
        var (min, max) = (Stats.CurStage.vertex_min, Stats.CurStage.vertex_max);
        values = new int[length];
        for (int i = 0; i < values.Length; ++i) {
            int j1 = (i + 1) % length, j2 = (i + length - 1) % length;
            do values[i] = UnityEngine.Random.Range(min, max + 1);
            while (values[i] == values[j1] || values[i] == values[j2]);
        }
    }
    public static void Clear() => values = null;
    public static void Replace(int idx, int val) {
        values[idx] = val;
    }
    public static bool SameTwoEndCheck(out List<int> idxs) {
        bool flag = false;
        idxs = new();
        for (int i = 0; i < Count; ++i) {
            if (values[i] == values[(i + Count - 1) % Count]) {
                // Debug.Log($"good idx: {i}");
                idxs.Add(i); flag = true;
            }
        }
        return flag;
    }
}

public static class PrizedEdges {
    private static Ability[] values;
    private static Ability RandomAbility() => (Ability)UnityEngine.Random.Range(0, (int)Ability.SIZE);
    public static void AdvInit() {
        values = new Ability[Vertexes.Count];
        int max_prized_edge = Mathf.RoundToInt(Vertexes.Count / 4f);
        // Debug.Log($"max_prized_edge:{max_prized_edge}");
        for (int i = 0; i < values.Length; ++i) {
            var rate = 1f - (float)max_prized_edge / (values.Length - i);
            var flag = UnityEngine.Random.value;
            // Debug.Log($"1-rate: {rate} ({(float)max_prized_edge / (values.Length - i)}) ? flag:{flag}");
            if (flag >= rate) {
                values[i] = RandomAbility();
                // Debug.Log($"prized idx: {i}");
                max_prized_edge--;
            }
            else values[i] = Ability.Null;
        }
    }
    public static void Clear() => values = null;
    public static bool IsPrizedEdge(int idx) => values[idx] != Ability.Null;
    public static Ability[] GetAbilities(int[] idxs) {
        var list = new List<Ability>();
        foreach (int idx in idxs) {
            if (values[idx] == Ability.Null) continue;
            list.Add(values[idx]);
        }
        return list.ToArray();
    }
}

public static class Edges {
    private static readonly List<Tuple<int, int>> edges = new();
    public static Tuple<int, int>[] EdgeList => edges.ToArray();
    private static readonly HashSet<Tuple<int, int>> set = new();
    public static Tuple<int, int> ActiveEdge {
        get {
            if (edges.Count == 0) return null;
            else return edges[active_index];
        }
    }
    private static int active_index = 0;
    public static void Clear() {
        edges.Clear(); set.Clear(); active_index = 0;
    }
    public static void AddEdge(Tuple<int, int> new_edge, bool forced_sum) {
        if (set.Contains(new_edge)) return;
        if (new_edge.Item1 == new_edge.Item2) return;
        if (forced_sum) {
            if (new_edge.Item1 + new_edge.Item2 > Stats.Point) return;
            Stats.Point -= new_edge.Item1 + new_edge.Item2;
        }
        set.Add(new_edge); edges.Add(new_edge); Refresh();
    }
    public static void RemoveEdge(Tuple<int, int> edge) {
        if (!set.Contains(edge)) return;
        set.Remove(edge); edges.Remove(edge); Refresh();
    }
    public static void ChangeActiveEdge(int dir) {
        active_index = (active_index + dir + edges.Count) % edges.Count;
    }
    public static void Refresh() {
        active_index = Mathf.Clamp(active_index, 0, edges.Count - 1);
        GameInterface.UpdateList();
    }

    public static void AddRandomEdge(int l = 0, int r = 0, bool cost = true) {
        var max = Stats.CurStage.vertex_max;
        List<Tuple<int, int>> list = new();

        int mint_l = l != 0 ? l : 1, maxt_l = l != 0 ? l : max,
            mint_r = r != 0 ? r : 1, maxt_r = r != 0 ? r : max;

        for (int t_l = mint_l; t_l <= maxt_l; ++t_l) {
            for (int t_r = mint_r; t_r <= maxt_r; ++t_r) {
                if (t_l + t_r > Stats.Point || t_l == t_r) continue;
                var t_edge = new Tuple<int, int>(t_l, t_r);
                if (set.Contains(t_edge)) continue;
                list.Add(t_edge);
            }
        }
        if (list.Count <= 0) return;
        AddEdge(list[UnityEngine.Random.Range(0, list.Count - 1)], cost);
    }
}

public static class Control {
    public static int target_edge;
    public static void Left() {
        target_edge = (target_edge + 1) % Vertexes.Count;
        GameDisplay.UpdatePlayer();
    }
    public static void Right() {
        target_edge = (target_edge + Vertexes.Count - 1) % Vertexes.Count;
        GameDisplay.UpdatePlayer();
    }
    public static Tuple<int, int> input_edge = new(0, 0);
    public static void Space() {
        int edge1 = target_edge, edge2 = (target_edge + Vertexes.Count - 1) % Vertexes.Count;
        if (Abilities.active == Ability.Pickup) {
            Edges.AddEdge(new(Vertexes.Vertex(edge1), Vertexes.Vertex(edge2)), false);
            Abilities.Resolve();
        }
        Tuple<int, int> edge = Edges.ActiveEdge;
        Edges.RemoveEdge(edge);
        Vertexes.Replace(edge1, edge.Item1); 
        Vertexes.Replace(edge2, edge.Item2);
        if (Vertexes.SameTwoEndCheck(out var vertexes)) {
            Stats.NextRound();
            Edges.AddRandomEdge(cost: false);
            var sum = vertexes.Sum(x => Vertexes.Vertex(x));
            if (Abilities.active == Ability.Profit) {
                sum *= 2; Abilities.Resolve();
            }
            Stats.Point += sum;
            foreach (Ability ablt in PrizedEdges.GetAbilities(vertexes.ToArray())) 
                Abilities.AddNewAbility(ablt);
            Vertexes.Clear(); Vertexes.AdvInit();
            PrizedEdges.Clear(); PrizedEdges.AdvInit();
            target_edge = Mathf.Clamp(target_edge, 0, Vertexes.Count);
            GameDisplay.MapCreate();
        }
        else GameDisplay.MapUpdate();
    }
    public static void Plus() {
        Edges.AddRandomEdge(input_edge.Item1, input_edge.Item2);
        input_edge = new(0, 0);
    }
    public static void Up() {
        Edges.ChangeActiveEdge(1);
        // GameDisplay.UpdatePlayer();
    }
    public static void Down() {
        Edges.ChangeActiveEdge(-1);
    }
    public static void EditInput_L(int l) {
        input_edge = new(l, input_edge.Item2);
    }
    public static void EditInput_R(int r) {
        input_edge = new(input_edge.Item1, r);
    }
    public static void Activate(Ability ability) {
        Abilities.Activate(ability);
        if (Abilities.active == Ability.Dismantle) {
            Tuple<int, int> edge = Edges.ActiveEdge;
            Edges.RemoveEdge(edge);
            Stats.Point += edge.Item1 + edge.Item2;
            Abilities.Resolve();
        }
    }
}

public static class Stats {
    [Serializable] public class Stages {
        public int min_round, vertex_min, vertex_max;
    }
    private static int round_cnt, stage_cnt;
    private static List<Stages> stages = new();
    public static int Point;
    public static void Init(List<Stages> _stages, int starting_point) {
        Point = starting_point;
        stages = _stages;
        round_cnt = stage_cnt = 0;
    }
    public static void NextRound() {
        round_cnt++;
        if (stage_cnt + 1 < stages.Count && round_cnt >= stages[stage_cnt + 1].min_round)
            stage_cnt++;
    }
    public static Stages CurStage => stages[stage_cnt];
    public static bool GameStart;
}