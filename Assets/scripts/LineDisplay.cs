using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LineDisplay: MonoBehaviour {
    [SerializeField] private TextMeshProUGUI idx;
    [SerializeField] private TextMeshProUGUI left, right;
    [SerializeField] private Image leftorb, rightorb, orbline;
    public void UpdateValue(int index, int lval, int rval) {
        if (idx != null) idx.text = index.ToString();
        left.text = lval <= 0 ? "" : lval.ToString();
        right.text = rval <= 0 ? "" : rval.ToString();
    }
    public void SetDisplayColor(Color color) {
        leftorb.color = rightorb.color = orbline.color = color;
    }
}