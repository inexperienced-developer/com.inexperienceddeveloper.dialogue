using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_interactionTypeText;
    [SerializeField] private TMP_Text m_interactableText;

    private void Start()
    {
        PlayerInteractor.CanInteract += OnCanInteract;
        PlayerInteractor.Interaction += OnInteract;
        PlayerInteractor.EndInteraction += OnEndInteract;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        PlayerInteractor.CanInteract -= OnCanInteract;
        PlayerInteractor.Interaction -= OnInteract;
        PlayerInteractor.EndInteraction -= OnEndInteract;
    }


    private void OnCanInteract(IInteractable interactable)
    {
        if (interactable == null) gameObject.SetActive(false);
        else
        {
            m_interactionTypeText.SetText(GetTypeInteraction(interactable));
            m_interactableText.SetText(interactable.transform.gameObject.name);
            gameObject.SetActive(true);
        }
    }

    private void OnInteract(IInteractable interactable, EActiveCamera cameraToTurnOn)
    {
        gameObject.SetActive(false);
    }

    private void OnEndInteract()
    {
        gameObject.SetActive(true);
    }

    private string GetTypeInteraction(IInteractable interactable)
    {
        return interactable switch
        {
            NPC => "Talk",
            _ => "Interact"
        };
    }
}
