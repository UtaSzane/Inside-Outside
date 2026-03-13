using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class GameInterface: MonoBehaviour {
    private static GameInterface instance;

    [Header("Others")]
    [SerializeField] [Min(1)] private int vertex_radius = 150;
    [SerializeField] [Min(1)] private int line_w = 15;
    [SerializeField] private RectTransform player;

    public class ObjectGroup<T> where T: MonoBehaviour {
        public readonly T obj;
        public readonly RectTransform prt;
        public readonly Stack<T> list, ulist;
        public ObjectGroup (T obj, RectTransform prt) {
            this.obj = obj; this.prt = prt;
            list = new(); ulist = new();
        }

        private void Normalize(int len) {
            while (list.Count > len) {
                T t_obj = list.Pop();
                ulist.Push(t_obj);

                t_obj.gameObject.name = "_";
                t_obj.gameObject.SetActive(false);
            }
            while (list.Count < len) {
                T t_obj;
                if (ulist.Count == 0) t_obj = Instantiate(obj, prt);
                else t_obj = ulist.Pop();
                list.Push(t_obj);
            }
        }
        public void UpdateObject(int len, Action<T, int> ObjectSetup) {
            Normalize(len);
            var t_arr = list.ToArray();
            for (int i = 0; i < t_arr.Length; ++i) {
                ObjectSetup(t_arr[i], i);
                t_arr[i].gameObject.name = i.ToString();
                t_arr[i].gameObject.SetActive(true);
            }
        }
        public void ClearObject() {
            Normalize(0);
        }
    }

    private static void ObjectSetup<T>(T obj, int idx) {
        if (obj is VertexObj vertex) {
            var radius = instance.vertex_radius;
            var playerpos = instance.player.position;
            var vertexcnt = GameLogic.VertexCount;
            var angle = Mathf.Deg2Rad * 360f / vertexcnt * (idx + 0.5f);
            vertex.transform.SetPositionAndRotation(
                new Vector2(
                    playerpos.x + radius * Mathf.Cos(angle),
                    playerpos.y + radius * Mathf.Sin(angle)
                ),
                Quaternion.identity
            );
            vertex.UpdateObject(idx);
        }
        else if (obj is EdgeObj edge) {
            var radius = instance.vertex_radius;
            var line_w = instance.line_w;
            var vertexcnt = GameLogic.VertexCount;
            var playerpos = instance.player.position;

            float rad = Mathf.PI * 2 / vertexcnt * idx;
            float width = radius * Mathf.Sin(Mathf.PI / vertexcnt) * 2f;
            float dist = radius * Mathf.Cos(Mathf.PI / vertexcnt);

            edge.transform.SetPositionAndRotation(
                new Vector2(
                    playerpos.x + dist * Mathf.Cos(rad),
                    playerpos.y + dist * Mathf.Sin(rad)
                ),
                Quaternion.Euler(Vector3.forward * 360f / vertexcnt * idx)
            );
            edge.GetComponent<RectTransform>().sizeDelta = new Vector3(line_w, width, 1f);
            edge.UpdateObject(idx);
        }
        else if (obj is PowerObj pwr) {
            pwr.UpdateObject(powerlist[idx]);
        }
        else if (obj is EdgeListObj edgel) {
            edgel.UpdateObject(edgelist[idx]);
        }
    }

    [Header("Vertexes")]
    [SerializeField] private VertexObj vertex_obj;
    [SerializeField] private RectTransform vertex_prt;
    private static ObjectGroup<VertexObj> vertexGroup;
    private static int[] vertexvalues = null;

    [Header("Edges")]
    [SerializeField] private EdgeObj edge_obj;
    [SerializeField] private RectTransform edge_prt;
    private static ObjectGroup<EdgeObj> edgeGroup;
    private static int vertexcount = -1;

    [Header("PowerList")]
    [SerializeField] private PowerObj power_obj;
    [SerializeField] private RectTransform power_prt;
    private static ObjectGroup<PowerObj> powerGroup;
    private static PowerClass[] powerlist = null;

    [Header("EdgeList")]
    [SerializeField] private EdgeListObj edgelist_obj;
    [SerializeField] private RectTransform edgelist_prt;
    private static ObjectGroup<EdgeListObj> edgelistGroup;
    private static Tuple<int, int>[] edgelist = null;

    private void Start() {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;

        vertexGroup   = new(instance.vertex_obj  , instance.vertex_prt  );
        edgeGroup     = new(instance.edge_obj    , instance.edge_prt    );
        powerGroup    = new(instance.power_obj   , instance.power_prt   );
        edgelistGroup = new(instance.edgelist_obj, instance.edgelist_prt);

        return;
    }

    private void Update1() {
        var t_vertexvalues = GameControl.VertexValues;
        if (vertexvalues != t_vertexvalues) {
            vertexvalues = t_vertexvalues;
            vertexGroup.UpdateObject(GameLogic.VertexCount, ObjectSetup);
        }

        var t_vertexcount = GameLogic.VertexCount;
        if (vertexcount != t_vertexcount) {
            vertexcount = t_vertexcount;
            edgeGroup.UpdateObject(GameLogic.VertexCount, ObjectSetup);
        }

        var t_powerlist = GameControl.GetPowerList;
        if (powerlist != t_powerlist) {
            powerlist = t_powerlist;
            powerGroup.UpdateObject(PowerList.Count, ObjectSetup);
        }

        var t_edgelist = GameControl.GetEdgeList;
        if (edgelist != t_edgelist) {
            edgelist = t_edgelist;
            edgelistGroup.UpdateObject(EdgeList.Count, ObjectSetup);
        }
    }
    public static void Clear() {
        vertexGroup.ClearObject();
        edgeGroup.ClearObject();
        powerGroup.ClearObject();
        edgelistGroup.ClearObject();
    }


    [Header("Others")]
    [SerializeField] private TextMeshProUGUI point_text;
    private static int point = -1;
    [SerializeField] private TextMeshProUGUI time_text;
    private static int time = -1;
    [SerializeField] private TextMeshProUGUI activepower;
    private static PowerClass power = PowerClass.Null;
    [SerializeField] private EdgeListObj activeedge;
    private static Tuple<int, int> aedge = null;

    private void Update2() {
        var t_point = GameLogic.Point;
        if (point != t_point) {
            point = t_point;
            point_text.text = point.ToString();
        }

        var t_time = GameControl.Time;
        if (t_time != time) {
            time = t_time;
            if (time <= 0) time_text.text = "---";
            else {
                string minutes = (time / 60).ToString().PadLeft(2, '0'),
                       seconds = (time % 60).ToString().PadLeft(2, '0');
                time_text.text = $"{minutes}:{seconds}";
            }
        }

        var t_power = GameControl.ActivePower;
        if (power != t_power) {
            power = t_power;
            activepower.text = power == PowerClass.Null ? "---" : power.ToString();
        }

        var t_edge = GameControl.ActiveEdge;
        if (aedge != t_edge) {
            aedge = t_edge;
            activeedge.UpdateObject(aedge);
        }
    }

    [Header("EdgeInput")]
    [SerializeField] private RectTransform inputrect;
    [SerializeField] private TMP_InputField lefti, righti;
    [SerializeField] private TextMeshProUGUI cost;
    private static bool inputonflag = true;

    public void EditInputL() {
        if (!GameControl.PreciseControl) return;
        if (!GameLogic.RoundStarted) return;
        var text = lefti.text == "" ? "0" : lefti.text;
        if (int.TryParse(text, out int lval) && lval > 0) {
            GameControl.Action_EditInputL(lval);
            cost.text = GameControl.InputEdgeCost.ToString();
        }
    }
    public void EditInputR() {
        if (!GameControl.PreciseControl) return;
        if (!GameLogic.RoundStarted) return;
        var text = righti.text == "" ? "0" : righti.text;
        if (int.TryParse(text, out int rval) && rval > 0) {
            GameControl.Action_EditInputR(rval);
            cost.text = GameControl.InputEdgeCost.ToString();
        }
    }
    public void AddEdge() {
        if (!GameLogic.RoundStarted) return;
        GameControl.Action_AddEdge();
        lefti.text = righti.text = "";
        cost.text = "";
    }

    private void Update() {
        if (!GameLogic.GameStarted) return;
        
        Update1(); Update2();
        if (inputonflag ^ GameControl.PreciseControl) inputonflag = GameControl.PreciseControl;
        inputrect.gameObject.SetActive(inputonflag);

        var vertexcnt = GameLogic.VertexCount;
        var taredge = GameControl.TargetEdgeIndex;

        if (vertexcnt == 0) player.eulerAngles = Vector3.zero;
        else player.eulerAngles = 360f * taredge * Vector3.forward / vertexcnt;
    }
}
