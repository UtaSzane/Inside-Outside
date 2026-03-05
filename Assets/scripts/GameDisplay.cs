using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameDisplay: MonoBehaviour {
    private static GameDisplay instance;
    [SerializeField] private RectTransform orbtemp, linetemp, player;
    [SerializeField] private RectTransform orbgroup, linegroup;
    private readonly List<RectTransform> orbs = new(), lines = new();
    [SerializeField] [Min(1)] private int orb_radius = 100, line_w = 10;
    
    private void Start() {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this; return;
    }

    public static void MapCreate() {
        if (instance == null) return;
        instance.Clear();
        var orbcnt = Gameplay.OrbValues.Count;
        for (int i = 0; i < orbcnt; ++i)
            instance.orbs.Add(instance.OrbCreate(i, orbcnt, Gameplay.OrbValues[i]));
        for (int i = 0; i < orbcnt; ++i)
            instance.lines.Add(instance.LineCreate(i, orbcnt));
    }
    public static void MapUpdate() {
        if (instance == null) return;
        var orbcnt = Gameplay.OrbValues.Count;
        for (int i = 0; i < orbcnt; ++i) 
            instance.orbs[i].GetComponent<OrbObj>().UpdateVal(Gameplay.OrbValues[i]);
    }

    public static void UpdatePlayer() {
        if (instance == null) return;
        var orbcnt = Gameplay.OrbValues.Count;
        instance.player.eulerAngles = 360f * Gameplay.PlayerPtr * Vector3.forward / orbcnt;
    }

    private void Clear() {
        for (int i = orbs.Count - 1; i >= 0; --i)
            DestroyImmediate(orbs[i].gameObject); orbs.Clear();
        for (int i = lines.Count - 1; i >= 0; --i)
            DestroyImmediate(lines[i].gameObject); lines.Clear();
    }

    private RectTransform OrbCreate(int index, int orbcnt, int value) {
        float angle = Mathf.Deg2Rad * 360f / orbcnt * (index + 0.5f);
        RectTransform neworb = Instantiate(
            orbtemp,
            (Vector2)player.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * orb_radius,
            Quaternion.identity,
            orbgroup.transform
        );
        neworb.GetComponent<OrbObj>().Init(value, index);
        neworb.gameObject.name = $"orb_{index}";
        return neworb;
    }
    private RectTransform LineCreate(int index, int orbcnt) {
        float deg = 360f / orbcnt * index, rad = deg * Mathf.Deg2Rad;
        float width = orb_radius * Mathf.Sin(Mathf.PI / orbcnt) * 2f;
        float dist = orb_radius * Mathf.Cos(Mathf.PI / orbcnt);
        RectTransform newline = Instantiate(
            linetemp,
            (Vector2)player.position + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * dist,
            Quaternion.Euler(Vector3.forward * deg),
            linegroup.transform
        );
        newline.sizeDelta = new Vector3(line_w, width, 1f);
        newline.gameObject.name = $"line_{index}";
        newline.GetComponent<LineObj>().Init(index);
        return newline;
    }
}