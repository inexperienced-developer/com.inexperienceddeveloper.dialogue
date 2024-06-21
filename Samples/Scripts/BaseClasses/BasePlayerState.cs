using System;

[Serializable]
public abstract class BasePlayerState
{
    protected PlayerStateMachine m_stateMachine;

    public virtual void OnEnter(PlayerStateMachine stateMachine)
    {
        m_stateMachine = stateMachine;
    }
    public abstract void OnUpdate(float deltaTime);
    public abstract void OnExit();
}

