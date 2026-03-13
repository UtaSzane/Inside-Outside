using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EdgeListObj: MonoBehaviour {
    [SerializeField] private TextMeshProUGUI left, right;
    [SerializeField] private Image lefti, righti, midi;
    private Tuple<int, int> my_val = null;
    public void UpdateObject(Tuple<int, int> val) {
        if (val == null) return;
        my_val = val;
        left.text  = val.Item1.ToString();
        right.text = val.Item2.ToString();
    }
    private void Update() {
        if (my_val == null) return;
        // if (!GameLogic.RoundStarted) return;

        bool flag = my_val == GameControl.ActiveEdge;
        lefti.color = righti.color = midi.color = flag ? Color.red : Color.white;
    }
}