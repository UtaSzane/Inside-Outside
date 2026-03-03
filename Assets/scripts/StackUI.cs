using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class StackUI : MonoBehaviour {
    private static StackUI instance;
    [SerializeField] private List<RectTransform> all_packs;
    [SerializeField] private RectTransform active_store, inactive_store, first;
    private readonly Stack<RectTransform> stack = new();
    
    private void Start() {
        if (instance != null) {Destroy(gameObject); return;}
        instance = this; Init(); Push(first); return; 
    }
    private void Init() {
        inactive_store.gameObject.SetActive(false);
        active_store.gameObject.SetActive(true);
        stack.Clear();
        foreach(RectTransform pack in all_packs)
            pack.SetParent(inactive_store);
    }
    public void Push(RectTransform pack) {
        if (all_packs.Contains(pack) && !stack.Contains(pack)) {
            stack.Push(pack); pack.SetParent(active_store);
        }
    }
    public void Pop() {
        stack.Pop().SetParent(inactive_store);
    }
}
