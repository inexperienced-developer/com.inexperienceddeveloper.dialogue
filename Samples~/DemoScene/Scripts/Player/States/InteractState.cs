using System;
using UnityEngine;

namespace InexperiencedDeveloper.Dialogue.Samples
{
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

}
