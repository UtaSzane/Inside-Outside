// using System.Collections.Generic;
// using UnityEngine;

// [DisallowMultipleComponent]
// public class GameDisplay: MonoBehaviour {
//     private static GameDisplay instance;
//     [SerializeField] private VertexObj vertextemp;
//     [SerializeField] private EdgeObj edgetemp;
//     [SerializeField] private RectTransform player;
//     [SerializeField] private RectTransform vertexgroup, edgegroup;
//     [SerializeField] [Min(1)] private int vertex_radius = 100, line_w = 10;

//     private readonly List<VertexObj> vertexes = new(), vertex_unused = new();
//     private readonly List<EdgeObj> edges = new(), edge_unused = new();
    
//     private void Start() {
//         if (instance != null) { Destroy(gameObject); return; }
//         instance = this; return;
//     }

//     public static void GameState_Generate() {
//         if (instance == null) return;
//         var vertexcnt = GameLogic.VertexCount;
//         instance.VertexInit(vertexcnt);
//         instance.EdgeInit(vertexcnt);
//     }
//     public static void GameState_Update() {
//         if (instance == null) return;
//         var vertexcnt = GameLogic.VertexCount;
//         for (int i = 0; i < vertexcnt; ++i) {
//             var obj = instance.vertexes[i].GetComponent<VertexObj>();
//             obj.UpdateVal(Vertexes.At(i));
//         }
//     }
//     public static void GameState_Clear() {
//         if (instance == null) return;
//         instance.VertexClear();
//         instance.EdgeClear();
//     }

//     public static void UpdatePlayer() {
//         if (instance == null) return;
//         var vertexcnt = GameLogic.VertexCount;
//         instance.player.eulerAngles = 360f * GameControl.TargettedEdgeIndex() * Vector3.forward / vertexcnt;
//     }

//     private void VertexInit(int vertexcnt) {
//         while (vertexes.Count > vertexcnt) {
//             var obj = vertexes[^1];
//             obj.gameObject.SetActive(false);
//             obj.gameObject.name = "_";
//             vertex_unused.Add(obj);
//             vertexes.Remove(obj);
//         }
//         for (int i = 0; i < vertexcnt; ++i) {
//             VertexObj obj;
//             if (i >= vertexes.Count) {
//                 if (vertex_unused.Count > 0) {
//                     obj = vertex_unused[0];
//                     obj.gameObject.SetActive(true);
//                     vertex_unused.Remove(obj);
//                 }
//                 else {
//                     obj = Instantiate(
//                         vertextemp,
//                         Vector2.zero, Quaternion.identity,
//                         vertexgroup.transform
//                     );
//                 }
//                 vertexes.Add(obj);
//             }
//             else obj = vertexes[i];
            
//             var value = Vertexes.At(i);
//             var angle = Mathf.Deg2Rad * 360f / vertexcnt * (i + 0.5f);
//             obj.transform.position = (Vector2)player.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * vertex_radius;
//             obj.GetComponent<VertexObj>().Init(value, i);
//             obj.gameObject.name = $"vertex_{i}";
//         }
//     }
//     private void VertexClear() {
//         for (int i = vertexes.Count - 1; i >= 0; --i) {
//             var obj = vertexes[i];
//             obj.gameObject.SetActive(false);
//             obj.gameObject.name = "_";
//             vertex_unused.Add(obj);
//             vertexes.Remove(obj);
//         }
//     }

//     private void EdgeInit(int vertexcnt) {
//         while (edges.Count > vertexcnt) {
//             var obj = edges[^1];
//             obj.gameObject.SetActive(false);
//             obj.gameObject.name = "_";
//             edge_unused.Add(obj);
//             edges.Remove(obj);
//         }
//         for (int i = 0; i < vertexcnt; ++i) {
//             EdgeObj obj;
//             if (i >= edges.Count) {
//                 if (edge_unused.Count > 0) {
//                     obj = edge_unused[0];
//                     obj.gameObject.SetActive(true);
//                     edge_unused.Remove(obj);
//                 }
//                 else {
//                     obj = Instantiate(
//                         edgetemp,
//                         Vector2.zero, Quaternion.identity,
//                         edgegroup.transform
//                     );
//                 }
//                 edges.Add(obj);
//             }
//             else obj = edges[i];
            
//             float deg = 360f / vertexcnt * i, rad = deg * Mathf.Deg2Rad;
//             float width = vertex_radius * Mathf.Sin(Mathf.PI / vertexcnt) * 2f;
//             float dist = vertex_radius * Mathf.Cos(Mathf.PI / vertexcnt);

//             obj.transform.SetPositionAndRotation(
//                 (Vector2)player.position + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * dist,
//                 Quaternion.Euler(Vector3.forward * deg)
//             );
//             obj.GetComponent<RectTransform>().sizeDelta = new Vector3(line_w, width, 1f);
//             obj.GetComponent<EdgeObj>().Init(i);
//             obj.gameObject.name = $"edge_{i}";
//         }
//     }
//     private void EdgeClear() {
//         for (int i = edges.Count - 1; i >= 0; --i) {
//             edges[i].gameObject.SetActive(false);
//             edges[i].gameObject.name = "_";
//             edge_unused.Add(edges[i]);
//             edges.Remove(edges[i]);
//         }
//     }
// }

// /*



// */