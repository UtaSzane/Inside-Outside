using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameEdge {
    public int Left { get; private set; }
    public int Right { get; private set; }
    public GameEdge(int left = 0, int right = 0) {
        if (left == 0 || right == 0 || left == right) throw new ArgumentException("invalid value");
        Left = left; Right = right;
    }
    public override string ToString() => $"L={Left}, R={Right}";
}

// public enum GameAbility { Null, Pickup, DoubleProfit, SIZE }

[DisallowMultipleComponent]
public class Gameplay: MonoBehaviour {
    private static Gameplay instance;
    [SerializeField] private int minval = 1, maxval = 10;
    private readonly List<GameEdge> lines = new();
    public static List<int> OrbVals { get; private set; }
    [SerializeField] private int actl_idx = 0;
    public static Vector2Int newl_val;
    public static int PCounter { get; private set; }
    public static int PlayerPtr { get; private set; }
    // public static HashSet<GameAbility> all_abilities;
    // public static GameAbility ActiveAbility { get; private set; }
    public static GameEdge ActL_Val() {
        if (instance == null) return null;
        if (!GameStart) return null;
        if (instance.lines == null) return null;
        if (instance.lines.Count == 0) return null;
        return instance.lines[instance.actl_idx];
    }
    public static GameEdge[] Lines() => instance.lines.ToArray();
    public static bool GameStart { get; private set; }

    private void Start() {
        if (instance != null) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject); instance = this;
        PlayerPtr = 0; PCounter = 100; GameStart = false;
        OrbVals = new();
        return;
    }

    private void Update() {
        if (!GameStart) {
            if (Input.GetKeyDown(KeyCode.Space)) LoadNewGame();
        }
        else {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
                var dir = Input.GetKeyDown(KeyCode.RightArrow) ? -1 : 1;
                PlayerPtr = (PlayerPtr + OrbVals.Count + dir) % OrbVals.Count;
                GameDisplay.UpdatePlayer();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
                if (lines == null || lines.Count == 0) return;
                var dir = Input.GetKeyDown(KeyCode.UpArrow) ? -1 : 1;
                actl_idx = (actl_idx + lines.Count + dir) % lines.Count;
            }
            
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (lines == null || lines.Count == 0) return;
                var lrval = UseLine();
                Replace(PlayerPtr, lrval.Left, lrval.Right);
                var prize = BoardCheck();
                PCounter += prize;
                if (BoardCheck() > 0) LoadNewGame();
                else {
                    GameDisplay.MapUpdate();
                    GameInterface.UpdateList();
                }
            }
        }
    }

    private int Rand() => UnityEngine.Random.Range(minval, maxval + 1);

    public static bool NewLine() {
        if (instance == null) return false;
        if (!GameStart) return false;
        if (newl_val.x <= 0 || newl_val.y <= 0) return false;
        return instance.CreateLine(newl_val.x, newl_val.y);
    }

    private int BoardCheck() {
        int sum = 0;
        if (OrbVals != null && OrbVals.Count > 0) {
            for (int i = 0; i < OrbVals.Count; ++i) {
                if (OrbVals[i] == OrbVals[(i + 1) % OrbVals.Count])
                    sum += OrbVals[i];
            }
        }
        return sum;
    }

    private void LoadNewGame() {
        StateGen();
        GameDisplay.MapCreate();
        if (!CreateLine()) Debug.LogWarning("WHAT");
        PlayerPtr = Mathf.Clamp(PlayerPtr, 0, OrbVals.Count - 1);
        GameDisplay.UpdatePlayer();
        GameStart = true;
    }

    private void Replace(int lidx, int lval, int rval) {
        if (!GameStart) return;
        if (lidx >= OrbVals.Count || lidx < 0) return;
        OrbVals[lidx] = lval;
        OrbVals[(lidx + OrbVals.Count - 1) % OrbVals.Count] = rval;
    }
    private bool CreateLine(int lval = 0, int rval = 0) {
        bool cost = true;
        if (lval == 0 && rval == 0) {
            while (lval == 0 || lval == rval) lval = Rand();
            while (rval == 0 || rval == lval) rval = Rand();
            cost = false;
        }
        if (lval == 0 || rval == 0) return false;
        if (lval == rval) return false;
        if (cost) {
            if (lval + rval > PCounter) return false;
            PCounter -= lval + rval;
        }
        lines.Add(new(lval, rval));
        GameInterface.UpdateList();
        return true;
    }
    private GameEdge UseLine() {
        if (lines == null) return null;
        var val = lines[actl_idx]; lines.RemoveAt(actl_idx);
        actl_idx = Mathf.Clamp(actl_idx, 0, Mathf.Max(lines.Count - 1, 0));
        return val;
    }

    private void StateGen() {
        int tp;
        do tp = UnityEngine.Random.Range(3, 13); while (tp == 7 || tp == 11);
        OrbVals.Clear();
        for (int i = 0; i < tp; ++i) {
            int val;
            do val = Rand();
            while(i > 0 && (val == OrbVals[i - 1] || (i == tp - 1 && val == OrbVals[0]))); 
            OrbVals.Add(val);
        }
    }
}