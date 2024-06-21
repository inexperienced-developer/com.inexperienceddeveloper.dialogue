using InexperiencedDeveloper.Dialogue;
using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCSO m_data;
    public NPCSO Data => m_data;

    [SerializeField] private UnityEvent[] m_events;

    private void Awake()
    {
        gameObject.name = m_data.Name;
        PlayerInteractor.EndInteraction += OnEndInteract;
    }

    private void OnDestroy()
    {
        PlayerInteractor.EndInteraction -= OnEndInteract;
    }

    public bool Interact()
    {
        return false;
    }

    private void OnEndInteract()
    {
    }

    public DialogueContainerSO GetCurrentNPCDialogue()
    {
        DialogueContainerSO dialogue = m_data.FirstTimeLines;
        return dialogue;
    }

    public void ThrowEvent(int index)
    {
        m_events[index]?.Invoke();
    }

    public string GetName()
    {
        return m_data.Name;
    }

    public void DemoEvent()
    {
        Debug.Log("Event thrown!");
    }
}

