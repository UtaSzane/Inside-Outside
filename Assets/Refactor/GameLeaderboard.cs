using System.Collections.Generic;
using System.Linq;
using Dan.Main;
using UnityEngine;

#pragma warning disable IDE0130
namespace Test {
#pragma warning restore IDE0130


    public class Entry {
        public readonly int Rank;
        public readonly string Username;
        public readonly int Score;
        public readonly bool Mine;
        public Entry(int rank, string name, int point, bool mine = false) {
            Rank = rank; Username = name; Score = point; Mine = mine;
        }
        public Entry(Dan.Models.Entry entry) : 
            this(entry.Rank, entry.Username, entry.Score, entry.IsMine()) {}
    }

    [DisallowMultipleComponent]
    public class GameLeaderboard: MonoBehaviour {
        private static GameLeaderboard instance;
        [SerializeField] private GameLeaderboardCache cache;
        public static readonly int EntryCount = 10;
        // private static Entry[] top10;
        // private static List<int> self_latest_10;
        // private static int max;
        // private static string username;
        
        private void Start() {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this; return;
        }
        public static void UploadEntry(int point)
        {
            bool upload = instance.cache.UploadEntry(point);

            if (upload) {
                var username = instance.cache.username;
                Leaderboards.leaderboard.UploadNewEntry(
                    username, point, 
                    success => {
                        if (success) LoadEntries(true);
                    },
                    error => {
                        Debug.LogWarning($"[GameLeaderboard]: {error}");
                    }
                );
            }
        }
        public static void LoadEntries(bool forced)
        {
            Leaderboards.leaderboard.GetEntries(
                entries => {
                    int idx = 0;
                    bool mine = !entries.Any(x => x.IsMine()), flag = false;
                    var t_top10 = new Entry[EntryCount];
                    for (int i = 0; i < entries.Length && idx < EntryCount; ++i)
                    {
                        if (mine && !flag)
                        {
                            if (entries[i].IsMine()) flag = true;
                            else if (idx == EntryCount - 1) continue;
                        }
                        t_top10[idx++] = new(entries[i]);
                    }
                    for (int i = idx; i < EntryCount; ++i)
                    {
                        t_top10[idx++] = new(0, "", 0);
                    }
                    instance.cache.UploadTop10(t_top10);
                }, 
                error => {
                    Debug.LogWarning($"[GameLeaderboard]: {error}");
                }
            );
        }
    }

    [CreateAssetMenu()]
    public class GameLeaderboardCache: ScriptableObject {
        public Entry[] top10;
        public List<int> self_latest_10;
        public int max;
        public string username;

        public void UploadTop10(Entry[] top10)
        {
            this.top10 = top10;
        }
        public bool UploadEntry(int point)
        {
            bool flag = point > max;
            max = Mathf.Max(max, point);
            if (self_latest_10.Count == 10) self_latest_10.RemoveAt(0);
            self_latest_10.Add(point);
            return flag;
        }
    }
}