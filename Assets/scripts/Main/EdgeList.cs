using System;
using System.Collections.Generic;

public static class EdgeList {
    private readonly static List<Tuple<int, int>> edges = new();
    public static int Count => edges.Count;
    public static void Init() {
        edges.Clear();
    }
    public static void Clear() {
        edges.Clear();
    }
    public static Tuple<int, int> Get(int idx) {
        return idx < 0 || idx >= edges.Count ? null : edges[idx];
    }
    public static Tuple<int, int> RemoveAt(int idx) {
        var _ = edges[idx];
        edges.RemoveAt(idx);
        return _;
    }
    public static void Remove(Tuple<int, int> edge) {
        edges.Remove(edge);
    }
    public static void Add(Tuple<int, int> edge) {
        edges.Add(edge);
    }
}
