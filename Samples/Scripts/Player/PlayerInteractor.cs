using Cinemachine;
using System;
using UnityEngine;

namespace InexperiencedDeveloper.Dialogue.Sample
{
    public class PlayerInteractor : MonoBehaviour
    {
        private Player m_player;

        // Static because only one interaction will happen at a time -- also eases the unsubscription
        // No need to go crazy here
        public static event Action<IInteractable> CanInteract;
        public static event Action<IInteractable, EActiveCamera> Interaction;
        public static event Action EndInteraction;

        [Tooltip("Range the raycast shoots")]
        [SerializeField] private float m_interactRange = 1;
        [Tooltip("The height of the head of the player")]
        [SerializeField] private float m_interactYOffset = 0.75f;
        [SerializeField] private LayerMask m_interactableLayer;

        private IInteractable m_targetInteractable;

        private bool m_isInteracting => m_player.StateMachine.CurrentState == EPlayerState.INTERACT;

        public void Init(Player player)
        {
            m_player = player;
        }

        private void Update()
        {
            CheckInteract();
            if (m_targetInteractable == null) return;
            if (Input.GetKeyDown(KeyCode.E))
            {
                NPC npc = m_targetInteractable as NPC;
                if (!m_isInteracting) Interact();
                else if (npc == null && m_isInteracting) EndInteract();
            }
        }

        private void CheckInteract()
        {
            if (m_isInteracting) return;
            Vector3 interactHeight = transform.position;
            interactHeight.y += m_interactYOffset;
            // Shoot raycast checking for interactables in front of us
            if (Physics.Raycast(interactHeight, transform.forward, out RaycastHit hit, m_interactRange, m_interactableLayer, QueryTriggerInteraction.Collide))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != m_targetInteractable)
                {
                    m_targetInteractable = interactable;
                    CanInteract?.Invoke(m_targetInteractable);
                }
            }
            else
            {
                if (m_targetInteractable != null)
                {
                    m_targetInteractable = null;
                    CanInteract?.Invoke(m_targetInteractable);
                }
            }
        }

        private void Interact()
        {
            m_targetInteractable.Interact();
            Interaction?.Invoke(m_targetInteractable, GetCameraToTurnOn(m_targetInteractable));
        }

        public void EndInteract()
        {
            EndInteraction?.Invoke();
            m_player.StateMachine.NextState(EPlayerState.MOVEMENT);
        }

        private EActiveCamera GetCameraToTurnOn(IInteractable interactable)
        {
            return interactable switch
            {
                NPC => EActiveCamera.NPC,
                _ => EActiveCamera.PLAYER
            };
        }
    }
}

