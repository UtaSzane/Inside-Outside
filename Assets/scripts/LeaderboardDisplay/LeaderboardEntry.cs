using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LeaderboardEntry: MonoBehaviour {
    [SerializeField] private TextMeshProUGUI standing, username, score;
    [SerializeField] private Image stdi, usri, pi;
    public static Color first  = new(1.00f, 0.82f, 0.11f), second = new(1f, 1f, 1f), third = new(0.64f, 0.26f, 0.04f);
    public void Load(int std, string name, int p, bool special) {
        bool flag = name == "";
        standing.text = std.ToString();
        username.text = flag ? "---" : name;
        score.text    = flag ? "---" : p.ToString();
        
        if (flag) {
            standing.color = username.color = score.color = Color.black;
            stdi.color = usri.color = pi.color = Color.gray;
        }
        else {
            var color = std == 1 ? first : (std == 2 ? second : (std == 3 ? third : Color.black));
            standing.color = username.color = score.color = color;
            if (special) stdi.color = usri.color = pi.color = Color.white;
        }
    }
}