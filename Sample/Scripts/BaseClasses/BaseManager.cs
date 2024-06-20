using UnityEngine;

public abstract class BaseManager : MonoBehaviour
{
    public abstract void Init();
    public abstract void CleanUp();
    public abstract void Restart();
}
