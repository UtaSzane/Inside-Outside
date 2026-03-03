using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class GameInterface: MonoBehaviour {
    private static GameInterface instance; 
    [SerializeField] private TextMeshProUGUI pcounter;
    [SerializeField] private RectTransform scrollview;
    [SerializeField] private TMP_InputField linp, rinp;
    [SerializeField] private GameLineDisplay actl_val;
    [SerializeField] private GameLineDisplay line_temp;
    private readonly List<RectTransform> curlist = new();

    private void Start() {
        if (instance != null) { DestroyImmediate(gameObject); return; }
        instance = this; return;
    }
    private void Update() {
        pcounter.text = $"pcounter: {Gameplay.PCounter}";

        var actl = Gameplay.ActL_Val();
        if (actl == null) actl_val.UpdateValue(-1, -1);
        else actl_val.UpdateValue(actl.Left, actl.Right);
    }

    public static void UpdateList() {
        if (instance == null) return;
        var vlist = Gameplay.Lines();
        while (instance.curlist.Count > vlist.Count()) {
            DestroyImmediate(instance.curlist[^1].gameObject);
            instance.curlist.RemoveAt(instance.curlist.Count - 1);
        }

        for (int i = 0; i < vlist.Count(); ++i) {
            if (i >= instance.curlist.Count) {
                var obj = Instantiate(
                    instance.line_temp.transform,
                    Vector2.zero, Quaternion.identity,
                    instance.scrollview
                ).GetComponent<RectTransform>();
                instance.curlist.Add(obj);
            }

            RectTransform rect = instance.curlist[i];
            rect.gameObject.name = $"line_display_{i}";
            rect.GetComponent<GameLineDisplay>().UpdateValue(vlist[i].Left, vlist[i].Right);
        }
    }

    public void UpdateLVal() {
        if (int.TryParse(linp.text, out int lval)) {
            if (lval <= 0) { Debug.LogWarning("l no positive = bad"); return; }
            Gameplay.newl_val.x = lval;
            Debug.Log($"lval = {lval}");
        }
        else Debug.LogWarning($"lvalt.text = '{linp.text}'");
    }
    public void UpdateRVal() {
        if (int.TryParse(rinp.text, out int rval)) {
            if (rval <= 0) { Debug.LogWarning("r no positive = bad"); return; }
            Gameplay.newl_val.y = rval;
            Debug.Log($"rval = {rval}");
        }
        else Debug.LogWarning($"rvalt.text = '{rinp.text}'");
    }

    public void NewLine() {
        if (Gameplay.NewLine()) linp.text = rinp.text = "";
        else Debug.LogWarning("bad");
    }
}