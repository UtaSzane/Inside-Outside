using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EdgeObj: MonoBehaviour {
    [SerializeField] private Image midi;
    private int my_idx = -1;
    private static Color Color_orange = new(1, 0.5f, 0.5f);
    public void UpdateObject(int i) {
        my_idx = i;
    }
    private void Update() {
        if (my_idx == -1) return;
        // if (!GameLogic.RoundStarted) return;
        
        midi.color = GameControl.TargetEdgeIndex == my_idx ? 
            (GameControl.PrizeView ? Color.yellow : Color.red  ) :
            (GameControl.PrizeView ? Color_orange : Color.white);
    }
}