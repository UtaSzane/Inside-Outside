using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable IDE0130
namespace Test {
#pragma warning restore IDE0130

    [DisallowMultipleComponent]
    public class GameControl: MonoBehaviour {
        [Serializable] public class GameStages { public int RoundMin; public int RoundMax; public int RoundReq; }
        private static GameControl instance;
        private static float time;
        private static int actedge_idx;
        private static int taredge_idx;
        private static bool paused = false;
        private static Tuple<int, int> edgeinp;
        private static int round_cnt;
        private static int stage_cnt;
        [SerializeField] private GameStages[] stages;
        private void Start() {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this; return;
        }

        private static void StartGame() {
            GameLogic.StartGame();
        }
        private static void EndGame() {
            GameLogic.StartGame();
        }
        public static void PauseGame() {
            paused = true;
        }
        public static void UnpauseGame() {
            paused = false;
        }
        private static void StartRound() {
            round_cnt += 1;
            if (stage_cnt + 1 < instance.stages.Length) {
                if (instance.stages[stage_cnt].RoundReq <= round_cnt)
                    stage_cnt += 1;
            }
            var stage = instance.stages[stage_cnt];
            GameLogic.StartRound(
                stage.RoundMin, stage.RoundMin, 0.2f, 100, 3
            );
        }
        private void Update()
        {
            if (!GameLogic.RoundStarted) return;
            if (paused) return;

            time = Mathf.Max(time - UnityEngine.Time.deltaTime, 0f);
            if (time == 0f) GameLogic.EndGame();

            if (Input.GetKeyDown(KeyCode.LeftArrow)) Action_KeyLeft();
            else if (Input.GetKeyDown(KeyCode.RightArrow)) Action_KeyRight(); 
            else if (Input.GetKeyDown(KeyCode.UpArrow)) Action_KeyUp();
            else if (Input.GetKeyDown(KeyCode.DownArrow)) Action_KeyDown();        
            else if (Input.GetKeyDown(KeyCode.Space)) Action_KeySpace();
        }
        private static void Action_KeyUp()
        {
            // Edges.ChangeActiveEdge(-1);
            var cnt = Test.EdgeList.Count;
            actedge_idx = (actedge_idx + cnt - 1) % cnt;
            throw new NotImplementedException();
        }
        private static void Action_KeyDown()
        {
            // Edges.ChangeActiveEdge(+1);
            var cnt = Test.EdgeList.Count;
            actedge_idx = (actedge_idx + 1) % cnt;
            throw new NotImplementedException();
        }
        private static void Action_KeyLeft()
        {
            // ChangePlayerTarget(+1);
            var cnt = GameLogic.VertexCount;
            taredge_idx = (taredge_idx + 1) % cnt;
            // throw new NotImplementedException();
        }
        private static void Action_KeyRight()
        {
            // ChangePlayerTarget(-1);
            var cnt = GameLogic.VertexCount;
            taredge_idx = (taredge_idx + cnt - 1) % cnt;
            // throw new NotImplementedException();
        }
        private static void Action_KeySpace()
        {
            GameLogic.ReplaceVertexValuesAtEdge(taredge_idx, Test.EdgeList.Get(actedge_idx));
            // throw new NotImplementedException();
        }
        public static void Action_EditInputL(int l)
        {
            edgeinp = new(l, edgeinp.Item2);
            // throw new NotImplementedException();
        }
        public static void Action_EditInputR(int r)
        {
            edgeinp = new(edgeinp.Item1, r);
            // throw new NotImplementedException();
        }
        public static void Action_AddEdge()
        {
            Test.EdgeList.Add(edgeinp);
            edgeinp = new(0, 0);
            // throw new NotImplementedException();
        }
        public static void Action_ActivatePower(PowerClass pwr)
        {
            GameLogic.ActivatePower(pwr);
            // throw new NotImplementedException();
        }
        public static int Time()
        {
            return Mathf.CeilToInt(time);
        }
        public static int Point()
        {
            return GameLogic.Point;
        }
        public static Tuple<int, int> ActiveEdge()
        {
            return Test.EdgeList.Get(actedge_idx);
        }
        public static PowerClass ActivePower()
        {
            return GameLogic.ActivePwr;
        }
        public static Tuple<int, int>[] EdgeList()
        {
            return GameLogic.Indexes.Select(x => Test.EdgeList.Get(x)).ToArray();
        }
        public static PowerClass[] PowerList()
        {
            return Test.PowerList.List();
        }
        public static int[] VertexValues()
        {
            return GameLogic.Indexes.Select(x => Vertexes.At(x)).ToArray();
        }
        public static bool[] PrizedEdges()
        {
            return GameLogic.Indexes.Select(x => EdgePrizes.Prized(x)).ToArray();
        }
        public static int ActiveEdgeIndex()
        {
            return actedge_idx;
        }
        public static int TargettedEdgeIndex()
        {
            return taredge_idx;
        }
    }
}