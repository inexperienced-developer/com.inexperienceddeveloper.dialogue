using UnityEngine;

public static class GameManagerUtils
{
    public static T AddManager<T>(Transform transform, string name) where T : MonoBehaviour
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);
        return go.AddComponent<T>();
    }
}
