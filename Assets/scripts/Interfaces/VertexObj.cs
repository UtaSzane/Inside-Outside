using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class VertexObj: MonoBehaviour {
    [SerializeField] private TextMeshProUGUI norm, next;
    [SerializeField] private Image midi;
    [SerializeField] private Animator animor;
    private int my_idx = -1;
    public void UpdateObject(int idx) {
        my_idx = idx;
        var val = Vertexes.At(idx);
        norm.text = val <= 0 ? norm.text : val.ToString();
    }
    private void Update() {
        if (my_idx == -1) return;
        
        var taredge = GameControl.TargetEdgeIndex;
        var vertexcnt = GameLogic.VertexCount;

        bool flag = my_idx == taredge || my_idx == (taredge + vertexcnt - 1) % vertexcnt;
        midi.color = flag ? Color.red : Color.white;
        if (flag) {
            var actedge = GameControl.ActiveEdge;
            if (actedge == null) flag = false;
            else next.text = (my_idx == taredge ? actedge.Item1 : actedge.Item2).ToString();
        }
        if (animor.GetBool("active") != flag) {
            animor.SetBool("active", flag);
        }
    }
}