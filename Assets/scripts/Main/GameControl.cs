using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameControl: MonoBehaviour {
    [Serializable] public class GameStages { 
        public int RoundMin; public int RoundMax; public int RoundReq;
        public GameStages(int req, int min, int max) {
            RoundMin = min; RoundMax = max; RoundReq = req;
        }
    }
    [SerializeField] [Min(0f)] private float MaxTime = 600f;
    [SerializeField] [Min(0)] private int StartPoint = 100;
    [SerializeField] [Range(0f, 1f)] private float PrizeRate = 0.2f;
    [SerializeField] [Min(1)] private int MaxLendTurn = 3;
    private static GameControl instance;
    private static float time;
    public static int ActiveEdgeIndex { get; private set; }
    public static int TargetEdgeIndex { get; private set; }
    private static bool paused = false;
    private static Tuple<int, int> edgeinp;
    private static int round_cnt;
    private static int stage_cnt;
    private static readonly GameStages[] stages = new GameStages[] {
        new(req:  0, min:   1, max:   3),
        new(req:  2, min:   1, max:   4),
        new(req:  5, min:   2, max:   6),
        new(req: 10, min:   3, max:   8),
        new(req: 15, min:   4, max:  10),
        
        new(req: 20, min:   5, max:  15),
        new(req: 22, min:   8, max:  17),
        new(req: 25, min:  10, max:  20),
        new(req: 30, min:  15, max:  25),
        new(req: 35, min:  20, max:  30),
        
        new(req: 40, min:  20, max:  40),
        new(req: 42, min:  25, max:  45),
        new(req: 45, min:  30, max:  52),
        new(req: 50, min:  35, max:  60),
        new(req: 55, min:  40, max:  70),
        
        new(req: 60, min:  40, max:  75),
        new(req: 62, min:  35, max:  80),
        new(req: 65, min:  30, max:  85),
        new(req: 70, min:  25, max:  90),
        new(req: 72, min:  20, max:  92),
        new(req: 74, min:  15, max:  94),
        new(req: 76, min:  10, max:  96),
        new(req: 78, min:   5, max:  98),
        new(req: 80, min:   1, max: 100),
    };

    public static bool PreciseControl { get; private set; }
    public static bool PrizeView { get; private set; }
    public static bool BetterLoot { get; private set; }
    private void Start() {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this; return;
    }

    public static void StartGame() {
        PreciseControl = false;
        PrizeView = false;
        BetterLoot = false;
        ActiveEdgeIndex = 0;
        TargetEdgeIndex = 0;
        round_cnt = -1;
        stage_cnt = 0;
        time = instance.MaxTime;
        
        GameLogic.StartGame(
            instance.StartPoint,
            instance.PrizeRate,
            instance.MaxLendTurn
        );
    }
    public static void EndGame() {
        GameLogic.EndGame();
        PreciseControl = false;
        PrizeView = false;
        ActiveEdgeIndex = 0;
        TargetEdgeIndex = 0;
    }
    public static void PauseGame() {
        paused = true;
    }
    public static void UnpauseGame() {
        paused = false;
    }

    // private static bool TEMP = false;
    public static void StartRound() {
        // if (!TEMP) return; TEMP = true;
        round_cnt += 1;
        if (stage_cnt + 1 < stages.Length) {
            if (stages[stage_cnt + 1].RoundReq <= round_cnt) {
                stage_cnt += 1;
                if (stage_cnt ==  5) {
                    Debug.LogWarning("Upgrade acquired: Precise Control");
                    PreciseControl = true;
                }
                if (stage_cnt == 10) {
                    Debug.LogWarning("Upgrade acquired: Prize Preview");
                    PrizeView = true;
                }
                if (stage_cnt == 15) {
                    Debug.LogWarning("Upgrade acquired: Better Loot Table");
                    BetterLoot = true;
                }
            }
        }
        var stage = stages[stage_cnt];
        GameLogic.StartRound(stage.RoundMin, stage.RoundMax);
        // TEMP = false;
    }
    private void Update() {
        if (!GameLogic.RoundStarted) {
            if (GameLogic.GameStarted) StartRound();
            return;
        }
        if (paused) return;

        GameLogic.ResolvePower(PowerClass.Upgrade);
        if (ActiveEdge != null) {
            var actedge = ActiveEdge;
            GameLogic.ResolvePower(
                PowerClass.Consume,
                actedge.Item1, actedge.Item2
            );
        }
        ActiveEdgeIndex = Mathf.Clamp(ActiveEdgeIndex, 0, Mathf.Max(EdgeList.Count - 1, 0));
        TargetEdgeIndex = Mathf.Clamp(TargetEdgeIndex, 0, Mathf.Max(GameLogic.VertexCount - 1, 0));

        time = Mathf.Max(time - UnityEngine.Time.deltaTime, 0f);
        if (time == 0f) GameLogic.EndGame();

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Action_KeyLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Action_KeyRight(); 
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Action_KeyUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Action_KeyDown();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) {
            Action_KeySpace();
        }
    }
    private static void Action_KeyUp() {
        var cnt = EdgeList.Count;
        // Debug.Log($"cnt = {cnt}");
        ActiveEdgeIndex = cnt == 0 ? 0 : (ActiveEdgeIndex + cnt - 1) % cnt;
        // Debug.Log($"actedgeidx = {ActiveEdgeIndex}");
    }
    private static void Action_KeyDown() {
        var cnt = EdgeList.Count;
        // Debug.Log($"cnt = {cnt}");
        ActiveEdgeIndex = cnt == 0 ? 0 : (ActiveEdgeIndex + 1) % cnt;
        // Debug.Log($"actedgeidx = {ActiveEdgeIndex}");
    }
    private static void Action_KeyLeft() {
        var cnt = GameLogic.VertexCount;
        TargetEdgeIndex = (TargetEdgeIndex + 1) % cnt;
    }
    private static void Action_KeyRight() {
        var cnt = GameLogic.VertexCount;
        TargetEdgeIndex = (TargetEdgeIndex + cnt - 1) % cnt;
    }
    private static void Action_KeySpace() {
        GameLogic.ReplaceVertexValuesAtEdge(
            TargetEdgeIndex, ActiveEdge
        );
    }
    public static void Action_EditInputL(int l) {
        edgeinp = new(l, edgeinp.Item2);
    }
    public static void Action_EditInputR(int r) {
        edgeinp = new(edgeinp.Item1, r);
    }
    public static void Action_AddEdge() {
        // Debug.Log("Hey that's not fair!?");
        GameLogic.AddEdgeToList(edgeinp);
        edgeinp = new(0, 0);
    }
    public static void Action_ActivatePower(PowerClass pwr){
        GameLogic.ActivatePower(pwr);
    }
    public static int Time { get {
        return Mathf.CeilToInt(time);
    }}
    public static int InputEdgeCost { get {
        return GameLogic.EdgeCost(edgeinp);
    }}
    public static Tuple<int, int> ActiveEdge { get {
        return EdgeList.Get(ActiveEdgeIndex);
    }}
    public static PowerClass ActivePower { get {
        return GameLogic.ActivePwr;
    }}
    public static Tuple<int, int>[] GetEdgeList { get {
        var list = new List<Tuple<int, int>>();
        for (int i = 0; i < EdgeList.Count; ++i) list.Add(EdgeList.Get(i));
        return list.ToArray();
    }}
    public static PowerClass[] GetPowerList { get {
        return PowerList.List();
    }}
    public static int[] VertexValues { get {
        var list = new List<int>();
        for (int i = 0; i < GameLogic.VertexCount; ++i) {
            var val = Vertexes.At(i);
            if (val == -1) return null;
            list.Add(val);
        }
        return list.ToArray();
    }}
    public static bool[] PrizedEdges { get {
        var list = new List<bool>();
        for (int i = 0; i < GameLogic.VertexCount; ++i) list.Add(EdgePrizes.Prized(i));
        return list.ToArray();
    }}
}
