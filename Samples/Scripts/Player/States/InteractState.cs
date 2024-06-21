using System;
using UnityEngine;

[Serializable]
public class InteractState : BasePlayerState
{
    public override void OnEnter(PlayerStateMachine stateMachine)
    {
        base.OnEnter(stateMachine);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public override void OnExit()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnUpdate(float deltaTime)
    {
    }
}
