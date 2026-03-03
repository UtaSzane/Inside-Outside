using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameLine: MonoBehaviour {
    [SerializeField] private int line_id;
    public void Init(int line_id) {
        this.line_id = line_id;
    }
    private void Update() {
        var plrptr = Gameplay.PlayerPtr;
        transform.GetComponent<Image>().color = line_id == plrptr ? Color.red : Color.white;
    }
}