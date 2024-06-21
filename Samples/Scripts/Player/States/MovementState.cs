using System;
using UnityEngine;

[Serializable]
public class MovementState : BasePlayerState
{
    public override void OnEnter(PlayerStateMachine stateMachine)
    {
        base.OnEnter(stateMachine);
    }

    public override void OnUpdate(float deltaTime)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = Run(deltaTime, horizontal, vertical);
        m_stateMachine.Controller.Move(move);
        Rotate();
        m_stateMachine.ApplyGravity();
    }

    public override void OnExit()
    {
    }

    private Vector3 Run(float deltaTime, float horizontal, float vertical)
    {
        Vector3 move = (horizontal * m_stateMachine.transform.right + vertical * m_stateMachine.transform.forward) * m_stateMachine.MoveSpeed * deltaTime;
        m_stateMachine.Anim.SetFloat("horizontal", horizontal);
        m_stateMachine.Anim.SetFloat("vertical", vertical);
        return move;
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        Vector3 targetRot = m_stateMachine.transform.eulerAngles;
        targetRot.y += mouseX * m_stateMachine.RotSpeed;
        m_stateMachine.transform.rotation = Quaternion.Euler(targetRot);
    }

}

