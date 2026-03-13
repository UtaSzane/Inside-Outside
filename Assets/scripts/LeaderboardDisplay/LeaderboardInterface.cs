using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class LeaderboardInterface: MonoBehaviour {
    [Header("Main")]
    [SerializeField] private bool WillLoadEntries = true;
    [SerializeField] private LeaderboardEntry[] entry_objs = new LeaderboardEntry[GameLeaderboard.EntryCount];
    [SerializeField] private TextMeshProUGUI is_loading_text;
    private static List<Entry> entries = null;
    private static bool loaded = true;
    public void LoadEntries(int meth) {
        // if (!WillLoadEntries) return;
        entries = null; loaded = false;
        // Debug.Log("LOL");
        GameLeaderboard.LoadEntries((LoadMethod)meth);
    }
    public static void RetrieveEntries(Entry[] _entries) {
        entries = _entries.ToList();
    }

    private void Update() {
        if (!loaded && entries != null) {
            for (int i = 0; i < GameLeaderboard.EntryCount; ++i) {
                entry_objs[i].Load(
                    entries[i].Rank, entries[i].Username, entries[i].Score, entries[i].Mine
                );
            }
            loaded = true;
        }
        if (!WillLoadEntries) return;
        is_loading_text.text = loaded ? "" : "Loading...";
    }

    
    [Header("Test")]
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField point;
    public void ChangeName() {
        if (!WillLoadEntries) return;
        GameLeaderboard.NewUsername(username.text);
    }
    public void TestUpload() {
        if (!WillLoadEntries) return;
        if (int.TryParse(point.text, out int p)) {
            Debug.Log($"{username.text}: {p}");
            GameLeaderboard.UploadEntry(p);
        }
        else Debug.LogWarning("");
    }
}