using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class PowerObj: MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;
    private PowerClass my_power = PowerClass.Null;
    public void UpdateObject(PowerClass pwr) {
        my_power = pwr;
        text.text = pwr.ToString();
    }
    public void Activate() {
        if (my_power == PowerClass.Null) return;
        // if (!GameLogic.RoundStarted) return;

        GameControl.Action_ActivatePower(my_power);
    }
}