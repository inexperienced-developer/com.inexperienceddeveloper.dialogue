using System;
using System.Collections.Generic;
using UnityEngine;

namespace InexperiencedDeveloper.Dialogue.Samples
{
    public enum EPlayerState
    {
        MOVEMENT,
        INTERACT,
    }

    public class PlayerStateMachine : MonoBehaviour
    {
        public CharacterController Controller { get; private set; }
        public Animator Anim { get; private set; }

        #region State related things
        private Dictionary<EPlayerState, BasePlayerState> m_states = new Dictionary<EPlayerState, BasePlayerState>();
        // Used for readability throughout
        public EPlayerState CurrentState { get; private set; }
        private BasePlayerState m_currentState;

        public void NextState(EPlayerState state)
        {
            if (m_currentState != null) m_currentState.OnExit();
            m_currentState = GetState(state);
            CurrentState = state;
            m_currentState.OnEnter(this);
        }

        private BasePlayerState GetState(EPlayerState state)
        {
            // State exists in dictionary
            if (m_states.TryGetValue(state, out BasePlayerState playerState)) return playerState;
            Action addStateToDict = state switch
            {
                EPlayerState.MOVEMENT => () => m_states.Add(EPlayerState.MOVEMENT, new MovementState()),
                EPlayerState.INTERACT => () => m_states.Add(EPlayerState.INTERACT, new InteractState()),
                _ => null
            };

            if (addStateToDict == null) return null;
            addStateToDict.Invoke();
            //Add based on the enum and return
            return m_states[state];
        }

        #endregion

        [Header("State variables")]
        [SerializeField] private float m_moveSpeed = 5f;
        public float MoveSpeed => m_moveSpeed;
        [SerializeField] private float m_rotSpeed = 10f;
        public float RotSpeed => m_rotSpeed;
        [SerializeField] private float m_climbSpeed = 2f;
        public float ClimbSpeed => m_climbSpeed;

        [Header("Gravity variables")]
        [SerializeField] private float m_gravity = 9.81f;
        [SerializeField] private float m_groundCheckRadius = 0.01f;
        [SerializeField] private LayerMask m_groundLayer;
        private Vector3 m_gravityVelocity;

        private void Awake()
        {
            // Start in the movement state
            NextState(EPlayerState.MOVEMENT);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Controller = GetComponent<CharacterController>();
            Anim = GetComponent<Animator>();

            Debug.Assert(Anim);
            Debug.Assert(Controller);

            PlayerInteractor.Interaction += OnInteract;
            PlayerInteractor.EndInteraction += OnEndInteract;
        }

        private void OnDestroy()
        {
            PlayerInteractor.Interaction -= OnInteract;
            PlayerInteractor.EndInteraction -= OnEndInteract;
        }

        private void OnInteract(IInteractable interactable, EActiveCamera cameraToTurnOn)
        {
            NextState(EPlayerState.INTERACT);
        }

        private void OnEndInteract()
        {
            NextState(EPlayerState.MOVEMENT);
        }

        private void Update()
        {
            if (m_currentState == null) NextState(EPlayerState.MOVEMENT);
            m_currentState.OnUpdate(Time.deltaTime);
        }

        public bool IsGrounded()
        {
            return Physics.CheckSphere(transform.position, m_groundCheckRadius, m_groundLayer);
        }

        public void ApplyGravity()
        {
            if (IsGrounded())
            {
                m_gravityVelocity.y = -2f;
            }
            else
            {
                m_gravityVelocity.y -= m_gravity * Time.deltaTime;
            }
            Controller.Move(m_gravityVelocity * Time.deltaTime);
        }
    }

}

