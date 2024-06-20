using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InexperiencedDeveloper.Dialogue.Sample
{
    public enum EActiveCamera
    {
        PLAYER,
        NPC
    }

    public class PlayerCameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera m_playerCamera;
        [SerializeField] private CinemachineVirtualCamera m_npcCamera;

        private Dictionary<EActiveCamera, CinemachineVirtualCamera> m_cameras = new Dictionary<EActiveCamera, CinemachineVirtualCamera>();
        public EActiveCamera EActiveCamera { get; private set; }
        public CinemachineVirtualCamera GetActiveCamera() => m_cameras[EActiveCamera];

        public void SwitchCamera(EActiveCamera camera)
        {
            if (camera == EActiveCamera) return;
            foreach (EActiveCamera key in m_cameras.Keys)
            {
                m_cameras[key].gameObject.SetActive(key == camera);
            }
            EActiveCamera = camera;
        }

        public void SwitchCamera(EActiveCamera camera, CinemachineCameraSettings settings)
        {
            SwitchCamera(camera);
            CinemachineVirtualCamera vcam = GetActiveCamera();
            CinemachineTransposer transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
            CinemachineComposer composer = vcam.GetCinemachineComponent<CinemachineComposer>();
            transposer.m_BindingMode = settings.BindingMode;
            transposer.m_FollowOffset = settings.FollowOffset;
            composer.m_TrackedObjectOffset = settings.TrackedObjectOffset;
        }

        private void Awake()
        {
            Debug.Assert(m_playerCamera);
            Debug.Assert(m_npcCamera);

            m_cameras.Add(EActiveCamera.PLAYER, m_playerCamera);
            m_cameras.Add(EActiveCamera.NPC, m_npcCamera);

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
            Action action = cameraToTurnOn switch
            {
                EActiveCamera.NPC => () => SwitchCameraToNPC(interactable),
                _ => () => SwitchCamera(EActiveCamera.PLAYER)
            };

            action();
        }

        private void SwitchCameraToNPC(IInteractable interactable)
        {
            NPC npc = (NPC)interactable;
            if (npc == null)
            {
                Debug.LogError($"Interactable ({interactable.transform.gameObject.name} is not an NPC");
                return;
            }
            SwitchCamera(EActiveCamera.NPC, npc.Data.GetCameraSettings());
        }

        private void OnEndInteract()
        {
            SwitchCamera(EActiveCamera.PLAYER);
        }
    }

    public struct CinemachineCameraSettings
    {
        public readonly CinemachineTransposer.BindingMode BindingMode;
        public readonly Vector3 FollowOffset;
        public readonly Vector3 TrackedObjectOffset;

        public CinemachineCameraSettings(CinemachineTransposer.BindingMode bindingMode, Vector3 followOffset, Vector3 trackedObjectOffset)
        {
            BindingMode = bindingMode;
            FollowOffset = followOffset;
            TrackedObjectOffset = trackedObjectOffset;
        }
    }
}

