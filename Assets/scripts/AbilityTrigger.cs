using TMPro;
using UnityEngine;
// using UnityEngine.UI;

[DisallowMultipleComponent]
public class AbilityTrigger: MonoBehaviour {
    [SerializeField] private TextMeshProUGUI ability_name;
    public void UpdateValue(Ability ability) {
        ability_name.text = ability.ToString();
    }
    public void Activate() => GameInterface.ActivateAbility(ability_name.text);
}