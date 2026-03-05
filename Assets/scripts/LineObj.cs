using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LineObj: MonoBehaviour {
    private int line_id;
    public void Init(int line_id) {
        this.line_id = line_id;
    }
    private void Update() {
        var plrptr = Gameplay.PlayerPtr;
        var img = transform.GetComponent<Image>();
        if (line_id == Gameplay.GiftedEdge) { 
        // if (Gameplay.PAbility == GameAbility.Greedy && line_id == Gameplay.GiftedLine) { 
            if (line_id == plrptr) img.color = Color.green;
            else img.color = Color.yellow;
        }
        else {
            if (line_id == plrptr) img.color = Color.red;
            else img.color = Color.white;
        }
    }
}