using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class GameLineDisplay: MonoBehaviour {
    [SerializeField] private TextMeshProUGUI left, right;
    public void UpdateValue(int lval, int rval) {
        left.text = lval <= 0 ? "" : lval.ToString();
        right.text = rval <= 0 ? "" : rval.ToString();
    }
}