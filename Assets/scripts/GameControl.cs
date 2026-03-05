using UnityEngine;

[DisallowMultipleComponent]
public class GameControl: MonoBehaviour {
    private static GameControl instance;
    private void Start() {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this; return;
    }

    private void Update() {
        if (!Gameplay.GameStart) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                // lines.Clear(); lineset.Clear(); abilities.Clear();
                // LoadNewGame();
            }
        }
        else {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
                // PlayerControl(Input.GetKeyDown(KeyCode.RightArrow));
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
                // if (lines == null || lines.Count == 0) return;
                // SelectActiveLine(Input.GetKeyDown(KeyCode.UpArrow));
            }
            
            if (Input.GetKeyDown(KeyCode.Space)) {
                // if (lines == null || lines.Count == 0) return;
                // DoWhateverThisIs();
            }
        }
    }
}