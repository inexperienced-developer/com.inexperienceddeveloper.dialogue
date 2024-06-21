using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInteractor Interactor { get; private set; }
    public PlayerCameraManager CameraManager { get; private set; }

    private void Awake()
    {
        StateMachine = GetComponent<PlayerStateMachine>();
        Interactor = GetComponent<PlayerInteractor>();
        CameraManager = GetComponent<PlayerCameraManager>();

        Interactor?.Init(this);
    }
}

