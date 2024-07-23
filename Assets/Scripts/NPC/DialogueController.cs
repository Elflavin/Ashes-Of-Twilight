using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject dialogueImage;
    [SerializeField] private GameObject optionButtons;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;
    [SerializeField, TextArea(4, 6)] private string[] extraLines;

    public float typingTime = 0.05f;

    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;
    private string[] currentDialogueLines;

    void Start()
    {
        // Inicializa las líneas de diálogo actuales
        currentDialogueLines = dialogueLines;
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!didDialogueStart)
            {
                StartDialogue();
            }
            else if (dialogueText.text == currentDialogueLines[lineIndex])
            {
                NextDialogueLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = currentDialogueLines[lineIndex];
            }
        }
    }

    private void StartDialogue()
    {
        // Verifica si se deben usar las lineas de dialogo adicionales
        if (this.gameObject.CompareTag("Oldman") && HeroStats.Instance.shortcutFounded)
        {
            currentDialogueLines = extraLines;
        }
        else
        {
            currentDialogueLines = dialogueLines;
        }

        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        lineIndex = 0;
        Time.timeScale = 0f;
        StartCoroutine(ShowLine());
    }

    public void NextDialogueLine()
    {
        lineIndex++;

        if (lineIndex == currentDialogueLines.Length - 1)
        {
            optionButtons.SetActive(true);
        }

        if (lineIndex < currentDialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            CloseDialogue();
        }
    }

    public void CloseDialogue()
    {
        didDialogueStart = false;
        optionButtons.SetActive(false);
        dialoguePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;

        foreach (char ch in currentDialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dialogueImage.SetActive(true);
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dialogueImage.SetActive(false);
            isPlayerInRange = false;
        }
    }
}
