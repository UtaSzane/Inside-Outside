using TMPro;
using UnityEngine;
// using UnityEngine.UI;

[DisallowMultipleComponent]
public class AbilityTrigger: MonoBehaviour {
    [SerializeField] private TextMeshProUGUI ability_name;
    public void UpdateValue(GameAbility ability) {
        ability_name.text = ability.ToString();
    }
    public void Activate() => GameInterface.ActivateAbility(ability_name.text);
}