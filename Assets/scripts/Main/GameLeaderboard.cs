using UnityEngine;
using Dan.Main;
using System.Linq;

public enum LoadMethod { LoadIfNeeded, AlwaysLoad, NoLoad }

[DisallowMultipleComponent]
public class GameLeaderboard: MonoBehaviour {
    private static GameLeaderboard instance;
    [SerializeField] private GameLeaderboardCache cache;
    public static readonly int EntryCount = 10;

    public static Entry[] Top10Entry {
        get {
            if (instance == null) return null;
            return instance.cache.top10;
        }
    }
    
    private void Start() {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        LeaderboardCreator.LoggingEnabled = false;
        return;
    }
    public static void UploadEntry(int point) {
        bool upload = instance.cache.UploadEntry(point);
        if (upload) {
            var username = instance.cache.username;
            Leaderboards.leaderboard.UploadNewEntry(
                username, point, 
                success => {
                    if (success) LoadEntries(LoadMethod.AlwaysLoad);
                },
                error => {
                    Debug.LogWarning($"[GameLeaderboard]: {error}");
                }
            );
        }
    }
    public static void LoadEntries(LoadMethod loadMeth) {
        bool flag = false;

        if (loadMeth == LoadMethod.NoLoad) flag = false;
        else if (loadMeth == LoadMethod.AlwaysLoad) flag = true;
        else {
            if (instance.cache.top10 == null || instance.cache.top10.Length < EntryCount) flag = true;
            else flag = false;
        }

        // Debug.Log(instance.cache.top10);
        if (flag) {
            Leaderboards.leaderboard.GetEntries(
                entries => {
                    int idx = 0;
                    bool mine = entries.Any(x => x.IsMine()), flag = false;
                    var t_top10 = new Entry[EntryCount];
                    for (int i = 0; i < EntryCount; ++i) {
                        t_top10[i] = new(i + 1, "", 0);
                    }
                    for (int i = 0; i < entries.Length && idx < EntryCount; ++i) {
                        if (mine && !flag) {
                            if (entries[i].IsMine()) flag = true;
                            else if (idx == EntryCount - 1) continue;
                        }
                        t_top10[idx++] = new(entries[i]);
                    }
                    if (idx > 0) {
                        instance.cache.UploadTop10(t_top10);
                        LeaderboardInterface.RetrieveEntries(t_top10);
                    }
                    return;
                }, 
                error => Debug.LogWarning($"[GameLeaderboard]: {error}")
            );
        }
        else LeaderboardInterface.RetrieveEntries(instance.cache.top10);
    }
    public static void NewUsername(string name) {
        instance.cache.username = name;
    }
}