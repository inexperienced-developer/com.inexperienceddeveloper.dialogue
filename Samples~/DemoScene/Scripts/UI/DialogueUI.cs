using InexperiencedDeveloper.Dialogue;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private Image m_npcImage;
    [SerializeField] private TMP_Text m_npcNameText;
    [SerializeField] private TMP_Text m_dialogueText;
    [SerializeField] private Button[] m_responseButtons;

    private NPC m_currentNpc;

    private Coroutine m_currentDialogueRoutine;
    private DialogueContainerSO m_currentDialogueContainer;
    private DialogueEvent m_currentDialogueEvent = null;

    private bool m_skip = false;
    private bool m_answered = false;

    private void Awake()
    {
        PlayerInteractor.Interaction += OnInteract;
        PlayerInteractor.EndInteraction += OnEndInteract;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        PlayerInteractor.Interaction -= OnInteract;
        PlayerInteractor.EndInteraction -= OnEndInteract;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !m_skip)
        {
            m_skip = true;
        }
    }

    private void OnInteract(IInteractable interactable, EActiveCamera cameraToTurnOn)
    {
        NPC npc = interactable as NPC;
        if (npc == null) return;
        m_currentNpc = npc;
        m_npcNameText.SetText(npc.GetName());
        // TODO: Add spot to get image
        // m_npcImage = 

        StartDialogue(npc.GetCurrentNPCDialogue());
    }

    private void StartDialogue(DialogueContainerSO container)
    {
        gameObject.SetActive(true);
        m_currentDialogueContainer = container;
        m_currentDialogueRoutine = StartCoroutine(SpeakWithNPC());
    }

    private void StopDialogue()
    {
        if (m_currentDialogueRoutine != null) StopCoroutine(m_currentDialogueRoutine);
        gameObject.SetActive(false);
    }

    private void OnEndInteract()
    {
        StopDialogue();
    }

    private IEnumerator SpeakWithNPC()
    {
        m_currentDialogueEvent = m_currentDialogueContainer.GetNextDialogueEvent();
        while (m_currentDialogueEvent.BaseNode != null)
        {
            bool answerButtonsActive = false;
            if (HandleEvent()) break;
            // If it's dialogue display text
            yield return DisplayText(m_currentDialogueEvent.BaseNode.Text);
            if (m_currentDialogueEvent.TargetNodes != null && m_currentDialogueEvent.TargetNodes.Length > 1)
            {
                answerButtonsActive = SetAnswerButtons(m_currentDialogueEvent.TargetNodes);
            }
            if (answerButtonsActive) yield return WaitForAnswer();
            else
            {
                while (true)
                {
                    yield return WaitForInput();
                    if (m_skip)
                    {
                        m_skip = false;
                        if (m_currentDialogueEvent.TargetNodes == null) m_currentDialogueEvent = new DialogueEvent();
                        else m_currentDialogueEvent = m_currentDialogueContainer.GetNextDialogueEvent(m_currentDialogueEvent.TargetNodes[0]);
                        break;
                    }
                }
            }
        }
        Debug.Log("dialogue over");
        EndDialogue();
    }

    private bool HandleEvent()
    {
        if (m_currentDialogueEvent.DialogueType == EDialogueType.EVENT)
        {
            m_currentNpc.ThrowEvent(int.Parse(m_currentDialogueEvent.BaseNode.Text));
            m_currentDialogueEvent = m_currentDialogueContainer.GetNextDialogueEvent(m_currentDialogueEvent.BaseNode);
            return true;
        }
        return false;
    }

    private IEnumerator DisplayText(string text)
    {
        m_dialogueText.text = "";
        text = text.Replace("\n", " ");
        List<char> fullDialogue = text.ToList();
        while (fullDialogue.Count > 0)
        {
            m_dialogueText.text += fullDialogue[0];
            fullDialogue.RemoveAt(0);
            yield return WaitForInput();
            if (m_skip)
            {
                m_skip = false;
                m_dialogueText.SetText(text);
                break;
            }
        }
    }

    private IEnumerator WaitForInput()
    {
        yield return new WaitForSeconds(0.05f);
    }

    private IEnumerator WaitForAnswer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            if (m_answered)
            {
                m_answered = false;
                SetAnswerButtons(null);
                break;
            }
        }
    }

    private bool SetAnswerButtons(DialogueSegment[] answerChoices)
    {
        if (answerChoices == null || answerChoices.Length == 0)
        {
            foreach (var btn in m_responseButtons) btn.gameObject.SetActive(false);
            return false;
        }
        for (int i = 0; i < m_responseButtons.Length; i++)
        {
            DialogueSegment node = answerChoices[i];
            m_responseButtons[i].onClick.RemoveAllListeners();
            m_responseButtons[i].onClick.AddListener(() =>
            {
                m_currentDialogueEvent = m_currentDialogueContainer.GetNextDialogueEvent(node);
                m_answered = true;
            });
            m_responseButtons[i].GetComponentInChildren<TMP_Text>().SetText(node.Text);
            m_responseButtons[i].gameObject.SetActive(true);
        }
        return true;
    }

    private void EndDialogue()
    {
        GameManager.Instance.PlayerManager.Player.Interactor.EndInteract();
        gameObject.SetActive(false);
    }
}
