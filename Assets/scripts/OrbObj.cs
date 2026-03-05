using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class OrbObj: MonoBehaviour {
    [SerializeField] private TextMeshProUGUI norm, next;
    private int orb_id = 0;
    public void Init(int value, int orb_id) {
        norm.text = next.text = $"{value}";
        this.orb_id = orb_id;
    }
    public void UpdateVal(int value) => norm.text = next.text = $"{value}";
    private void Update() {
        var orbcnt = Vertexes.Count;
        if (orbcnt == 0) return;
        var plrptr = Control.target_edge;
        var flag = orb_id == plrptr || orb_id == (plrptr + orbcnt - 1) % orbcnt;
        transform.GetComponent<Image>().color = flag ? Color.red : Color.white;
    }
}