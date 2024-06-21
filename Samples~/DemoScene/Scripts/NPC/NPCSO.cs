using Cinemachine;
using InexperiencedDeveloper.Dialogue;
using UnityEngine;

namespace InexperiencedDeveloper.Dialogue.Samples
{
    [CreateAssetMenu(menuName = "NPC/Data", fileName = "New NPC Data")]
    public class NPCSO : ScriptableObject
    {
        [SerializeField] private string m_name;
        public string Name => m_name;

        [Header("Dialogue Settings")]
        [SerializeField] private DialogueContainerSO m_firstTimeLines;
        public DialogueContainerSO FirstTimeLines => m_firstTimeLines;

        [Header("Camera Focus Settings")]
        [SerializeField] private CinemachineTransposer.BindingMode m_bindingMode;
        [SerializeField] private Vector3 m_followOffset;
        [SerializeField] private Vector3 m_trackedObjectOffset;
        public CinemachineCameraSettings GetCameraSettings() => new CinemachineCameraSettings(m_bindingMode, m_followOffset, m_trackedObjectOffset);
    }
}

