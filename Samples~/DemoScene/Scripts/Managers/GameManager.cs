using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerManager PlayerManager { get; private set; }

    protected void Awake()
    {
        Instance = this;

        PlayerManager ??= GameManagerUtils.AddManager<PlayerManager>(transform, "PlayerManager");

        PlayerManager?.Init();
    }

    protected void OnDestroy()
    {
        PlayerManager?.CleanUp();
    }
}

