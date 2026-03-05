using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
// using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameInterface: MonoBehaviour {
    private static GameInterface instance; 
    [SerializeField] private TextMeshProUGUI pcounter;
    [SerializeField] private TextMeshProUGUI active_ability;
    [SerializeField] private RectTransform scrollview;
    [SerializeField] private LineDisplay active_line_display;
    [SerializeField] private LineDisplay line_temp;
    [SerializeField] private RectTransform newl_input;
    [SerializeField] private RectTransform abilityview;
    [SerializeField] private AbilityTrigger abiliy_temp;

    private readonly List<LineDisplay> curlistd = new();
    private readonly List<AbilityTrigger> curalistd = new();

    private void Start() {
        if (instance != null) { DestroyImmediate(gameObject); return; }
        instance = this; return;
    }
    private void Update() {
        pcounter.text = Gameplay.Point.ToString();

        var abl = Gameplay.ActiveAbility;
        active_ability.text = abl == GameAbility.Null ? "---" : abl.ToString();

        var actl = Gameplay.ActiveEdge();
        if (actl == null) {
            active_line_display.UpdateValue(-1, -1, -1);
            return;
        }

        bool flag1 = newl_input.gameObject.activeInHierarchy,
             flag2 = Gameplay.ActiveAbility == GameAbility.Control;

        if (flag1 != flag2) {
            Debug.Log($"flag1: {flag1}, flag2: {flag2}");
            newl_input.gameObject.SetActive(flag2);
        }
        
        var true_actl = actl;
        active_line_display.UpdateValue(-1, true_actl.Item1, true_actl.Item2);

        var list = Gameplay.Edges();
        for (int i = 0; i < curlistd.Count(); ++i) 
            curlistd[i].SetDisplayColor(list[i] == true_actl ? Color.red : Color.white);
    }

    public static void UpdateAbilityList() {
        if (instance == null) return;
        var alist = Gameplay.Abilities();
        while (instance.curalistd.Count > alist.Length) {
            DestroyImmediate(instance.curalistd[^1].gameObject);
            instance.curalistd.RemoveAt(instance.curalistd.Count - 1);
        }
        
        for (int i = 0; i < alist.Length; ++i) {
            if (i >= instance.curalistd.Count) { 
                instance.curalistd.Add(Instantiate(
                    instance.abiliy_temp,
                    Vector2.zero, Quaternion.identity,
                    instance.abilityview
                ));
            }

            var rect = instance.curalistd[i];
            rect.gameObject.name = $"ability_{i}";
            rect.UpdateValue(alist[i]);
        }
    }
    public static void UpdateList() {
        if (instance == null) return;
        var vlist = Gameplay.Edges();
        while (instance.curlistd.Count > vlist.Length) {
            DestroyImmediate(instance.curlistd[^1].gameObject);
            instance.curlistd.RemoveAt(instance.curlistd.Count - 1);
        }

        for (int i = 0; i < vlist.Length; ++i) {
            if (i >= instance.curlistd.Count) { 
                instance.curlistd.Add(Instantiate(
                    instance.line_temp,
                    Vector2.zero, Quaternion.identity,
                    instance.scrollview
                ));
            }

            var rect = instance.curlistd[i];
            rect.gameObject.name = $"line_display_{i}";
            rect.UpdateValue(i + 1, vlist[i].Item1, vlist[i].Item2);
        }
    }
    // public void NewLine() => Gameplay.NewLine();

    // public void ActivateAbility(TextMeshProUGUI code) {
    //     if (System.Enum.TryParse(code.text, false, out GameAbility res))
    //         Gameplay.ActivateAbility(res);
    //     else Debug.Log("bugged");
    // }
    public static void ActivateAbility(string code) {
        if (System.Enum.TryParse(code, false, out GameAbility res))
            Gameplay.ActivateAbility(res);
        else Debug.Log("bugged");
    }

    [SerializeField] private TMP_InputField linp, rinp;
    public void UpdateLVal() {
        var text = linp.text == "" ? "0" : linp.text;
        if (int.TryParse(text, out int lval) && lval > 0) {
            Gameplay.newl_val = new(lval, Gameplay.newl_val.Item2);
        }
        else Debug.LogWarning($"lvalt.text = '{text}'");
    }
    public void UpdateRVal() {
        var text = rinp.text == "" ? "0" : rinp.text;
        if (int.TryParse(text, out int rval) && rval > 0) {
            // Gameplay.newl_val.Item2 = rval;
            Gameplay.newl_val = new(Gameplay.newl_val.Item1, rval);
        }
        else Debug.LogWarning("bad value");
    }
    public void NewLine() {
        if (Gameplay.CreateEdge()) linp.text = rinp.text = "";
        else Debug.LogWarning("bad value");
    }
}