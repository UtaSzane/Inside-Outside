/*
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameAbility { Null, Pickup, Profit, Dismantle, Greedy, Control, SIZEP1 }

[DisallowMultipleComponent]
public class Gameplay: MonoBehaviour {
    [Serializable] public class GameStage {
        public int min_round_win, orb_min_value, orb_max_value;    
    }

    private static Gameplay instance;
    [SerializeField] private List<GameStage> gameStages;
    private readonly List<Tuple<int, int>> edges = new();
    private readonly HashSet<Tuple<int, int>> edgeset = new();
    public static List<int> OrbValues { get; private set; }
    private int actl_idx = 0;
    private int roundcnt = 0;
    private int stagecnt = 0;
    public static Tuple<int, int> newl_val;
    public static int Point { get; private set; }
    public static int PlayerPtr { get; private set; }
    public static GameAbility ActiveAbility { get; private set; }
    private static readonly int[] valid_orbcnt = new int[] {5, 6, 8, 9, 10, 12};
    public static int GiftedEdge { get; private set; }
    public static GameAbility GiftedAbility { get; private set; }
    private readonly HashSet<GameAbility> abilities = new();
    public static bool GameStart { get; private set; }

    private GameStage CurrentStage() {
        return gameStages[stagecnt];
    }

    public static Tuple<int, int> ActiveEdge() {
        if (instance == null) return null;
        if (!GameStart) return null;
        if (instance.edges == null) return null;
        if (instance.edges.Count == 0) return null;
        return instance.edges[instance.actl_idx];
    }
    public static Tuple<int, int>[] Edges() => instance.edges.ToArray();
    public static GameAbility[] Abilities() => instance.abilities.ToArray();

    private void Start() {
        if (instance != null) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject); instance = this;
        PlayerPtr = 0; Point = 100; GameStart = false;
        ActiveAbility = GameAbility.Null;
        OrbValues = new();
        newl_val = new(0, 0);
        return;
    }

    private void Update() {
        if (!GameStart) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                edges.Clear(); edgeset.Clear(); abilities.Clear();
                LoadNewRound();
            }
        }
        else {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
                ControlPlayer(Input.GetKeyDown(KeyCode.RightArrow));
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
                SelectActiveEdge(Input.GetKeyDown(KeyCode.UpArrow));
            }
            
            if (Input.GetKeyDown(KeyCode.Space)) {
                ConfirmAction();
            }
        }
    }

    private void ControlPlayer(bool isright) {
        var dir = isright ? -1 : 1;
        PlayerPtr = (PlayerPtr + OrbValues.Count + dir) % OrbValues.Count;
        GameDisplay.UpdatePlayer();
    }
    private void SelectActiveEdge(bool isup) {
        if (edges == null || edges.Count == 0) return;
        var dir = isup ? -1 : 1;
        actl_idx = (actl_idx + edges.Count + dir) % edges.Count;
    }
    private void ConfirmAction() {
        if (edges == null || edges.Count == 0) return;
        
        var lrval = RemoveEdge();
        if (lrval == null) return;

        var oldl = Replace(PlayerPtr, lrval.Item1, lrval.Item2);
        if (oldl == null) return;

        var prize = RoundWinCheck();
        int mult = 1;
        if (ActiveAbility == GameAbility.Profit || ActiveAbility == GameAbility.Pickup || ActiveAbility == GameAbility.Greedy) {
            if (ActiveAbility == GameAbility.Profit) mult = 2;
            if (ActiveAbility == GameAbility.Pickup) Core_CreateEdge(oldl.Item1, oldl.Item2, false);
            ActiveAbility = GameAbility.Null;
        }
        Point += mult * prize;
        newl_val = new(0, 0);

        if (prize > 0) {
            roundcnt++;
            if (stagecnt + 1 < gameStages.Count &&
                gameStages[stagecnt + 1].min_round_win <= roundcnt)
                stagecnt++;
            LoadNewRound();
        }
        else {
            GameDisplay.MapUpdate();
            GameInterface.UpdateList();
        }
    }

    public static bool CreateEdge() {
        if (instance == null) return false;
        if (!GameStart) return false;
        var flag = instance.Core_CreateEdge(newl_val.Item1, newl_val.Item2, cost: true);
        if (flag && ActiveAbility == GameAbility.Control) ActiveAbility = GameAbility.Null;
        return flag;
    }
    public static bool ActivateAbility(GameAbility ability) {
        if (instance == null) return false;
        if (ability == GameAbility.Null || ability == GameAbility.SIZEP1) return false;
        if (!instance.abilities.Contains(ability)) return false;
        if (ability == GameAbility.Dismantle) {
            var lrval = instance.RemoveEdge();
            if (lrval == null) return false;
            Point += lrval.Item1 + lrval.Item2;
            GameInterface.UpdateList();
        }
        else ActiveAbility = ability;
        if (!instance.RemoveAbility(ability)) Debug.LogWarning("HUH???");
        return true;
    }

    private int RoundWinCheck() {
        int sum = 0;
        if (OrbValues != null && OrbValues.Count > 0) {
            for (int i = 0; i < OrbValues.Count; ++i) {
                if (OrbValues[i] == OrbValues[(i + 1) % OrbValues.Count]) {
                    sum += OrbValues[i];
                    if ((i + 1) % OrbValues.Count == GiftedEdge) {
                        if (!AddAbility(GiftedAbility)) 
                            Debug.LogWarning($"Ability {GiftedAbility} already existed (?)");
                    }   
                }
            }
        }
        return sum;
    }

    private void LoadNewRound() {
        RoundStateGen();
        GameDisplay.MapCreate();
        Core_CreateEdge(cost: false);
        PlayerPtr = Mathf.Clamp(PlayerPtr, 0, OrbValues.Count - 1);
        GameDisplay.UpdatePlayer();

        GiftedEdge = UnityEngine.Random.Range(0, OrbValues.Count);
        GiftedAbility = (GameAbility)UnityEngine.Random.Range(
            (int)GameAbility.Pickup, (int)GameAbility.SIZEP1
        );
        
        GameStart = true;
    }

    private bool AddAbility(GameAbility ability) {
        if (ability == GameAbility.Null || ability == GameAbility.SIZEP1) return false;
        if (!abilities.Add(ability)) return false;
        GameInterface.UpdateAbilityList();
        return true;
    }
    private bool RemoveAbility(GameAbility ability) {
        if (ability == GameAbility.Null || ability == GameAbility.SIZEP1) return false;
        if (!abilities.Remove(ability)) return false;
        GameInterface.UpdateAbilityList();
        return true;
    }

    private Tuple<int, int> Replace(int lidx, int lval, int rval) {
        if (!GameStart) return null;
        if (lidx >= OrbValues.Count || lidx < 0) return null;
        Tuple<int, int> old = new(OrbValues[lidx], OrbValues[(lidx + OrbValues.Count - 1) % OrbValues.Count]);
        OrbValues[lidx] = lval;
        OrbValues[(lidx + OrbValues.Count - 1) % OrbValues.Count] = rval;
        return old;
    }
    private bool Core_CreateEdge(int lval = 0, int rval = 0, bool cost = true) {
        var t_newl = EdgeGenerate(lval, rval, cost);
        if (t_newl == null) return false;
        var newl = t_newl;

        if (!IsEdgeValid(newl.Item1, newl.Item2, cost, false)) return false;
        if (!edgeset.Contains(newl)) {
            if (cost) Point -= newl.Item1 + newl.Item2;
            edges.Add(newl); edgeset.Add(newl);
            GameInterface.UpdateList();
        }
        return true;
    }

    private Tuple<int, int> RemoveEdge() {
        if (edges == null) return null;
        if (edges.Count == 0) return null;
        var val = edges[actl_idx];
        edges.Remove(val); edgeset.Remove(val);
        actl_idx = Mathf.Clamp(actl_idx, 0, Mathf.Max(edges.Count - 1, 0));
        return val;
    }

    private bool IsEdgeValid(int lval, int rval, bool cost = true, bool funiq = true) {
        if (funiq && edgeset.Contains(new(lval, rval))) return false;
        if (lval <= 0 || rval <= 0 || lval == rval) return false;
        if (cost && lval + rval > Point) return false;
        return true;
    }

    private Tuple<int, int> EdgeGenerate(int lval = 0, int rval = 0, bool cost = true) {
        List<Tuple<int, int>> valid_edges = new();
        
        var curstg = CurrentStage();
        Range std = new((lval != 0 || rval != 0) ? 1 : curstg.orb_min_value, curstg.orb_max_value);
        Range lrng = lval != 0 ? new(lval, lval) : std;
        Range rrng = rval != 0 ? new(rval, rval) : std;

        for (int l = lrng.Start.Value; l <= lrng.End.Value; ++l) {
            for (int r = rrng.Start.Value; r <= rrng.End.Value; ++r) {
                if (IsEdgeValid(l, r, cost, true)) valid_edges.Add(new(l, r));
            }
        }

        if (valid_edges.Count == 0) {
            if (lval != 0 && rval != 0) return new(lval, rval);
            return null;
        }
        else {
            var idx = UnityEngine.Random.Range(0, valid_edges.Count);
            return valid_edges[idx];
        }
    }

    private void RoundStateGen() {
        OrbValues.Clear();
        int tp = valid_orbcnt[UnityEngine.Random.Range(0, valid_orbcnt.Length)];
        var curstg = CurrentStage();
        for (int i = 0; i < tp; ++i) {
            int val;
            do val = UnityEngine.Random.Range(curstg.orb_min_value, curstg.orb_max_value + 1);
            while (i > 0 && (val == OrbValues[i - 1] || (i == tp - 1 && val == OrbValues[0]))); 
            OrbValues.Add(val);
        }
    }
}
*/