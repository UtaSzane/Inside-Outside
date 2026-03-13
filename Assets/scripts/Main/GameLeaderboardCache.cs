using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class Entry {
    public string Username;
    public int Rank;
    public int Score;
    public bool Mine;
    public Entry(int rank, string name, int point, bool mine = false) {
        Rank = rank; Username = name; Score = point; Mine = mine;
    }
    public Entry(Dan.Models.Entry entry) : 
        this(entry.Rank, entry.Username, entry.Score, entry.IsMine()) {}
}

[CreateAssetMenu(fileName = "Cache", menuName = "ScriptableObjects/GameLeaderboardCache")]
public class GameLeaderboardCache: ScriptableObject {
    public string username;
    public Entry[] top10;
    public List<int> self_latest_10;
    public int max;

    public void UploadTop10(Entry[] top10) {
        this.top10 = top10;
    }
    public bool UploadEntry(int point) {
        bool flag = point > max;
        max = Mathf.Max(max, point);
        if (self_latest_10.Count == 10) self_latest_10.RemoveAt(0);
        self_latest_10.Add(point);
        return flag;
    }
}
