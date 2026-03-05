using UnityEngine;

public class GameControl: MonoBehaviour {
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (!Stats.GameStart) Gameplay.GameStart();
            else Gameplay.GameEnd();
        }
    }
}