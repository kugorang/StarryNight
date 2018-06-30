using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplayer : MonoBehaviour
{
    public GameObject dialougeWindowHolder;

    private string m_currentTypingDialgoue;
    public Text nameDisplayer, sayDisplayer;

    [Range(0.0f, 1.0f)] public float timeBetUpdateLetters;

    public bool IsTyping { get; private set; }

    public void ShowDialogueHolder()
    {
        dialougeWindowHolder.SetActive(true);
    }

    public void HideDialogueHolder()
    {
        dialougeWindowHolder.SetActive(false);
    }

    // Skip and Complete Current Dialogues
    public void SkipTypingLetter()
    {
        StopCoroutine("TypeText");
        StopCoroutine("TypeAndAddText");

        IsTyping = false;

        sayDisplayer.text = m_currentTypingDialgoue;
    }

    public void SetSay(string dialogue)
    {
        m_currentTypingDialgoue = dialogue;

        if (timeBetUpdateLetters <= 0f)
        {
            sayDisplayer.text = dialogue;
        }
        else
        {
            gameObject.SetActive(true);
            StartCoroutine("TypeText", m_currentTypingDialgoue);
        }
    }

    public void SetSay(string speakerName, string dialogue)
    {
        nameDisplayer.text = speakerName;
        m_currentTypingDialgoue = dialogue;

        if (timeBetUpdateLetters <= 0f)
            sayDisplayer.text = dialogue;
        else
            StartCoroutine("TypeText", m_currentTypingDialgoue);
    }

    // Update Text from buffer
    public IEnumerator TypeText(string texts)
    {
        IsTyping = true;

        ShowDialogueHolder();

        sayDisplayer.text = string.Empty;

        foreach (var letter in texts)
        {
            sayDisplayer.text += letter;
            yield return new WaitForSeconds(timeBetUpdateLetters);
        }

        IsTyping = false;
    }

    // Add Text, not replace 
    public IEnumerator TypeAndAddText(string texts)
    {
        IsTyping = true;

        ShowDialogueHolder();

        foreach (var letter in texts)
        {
            sayDisplayer.text += letter;
            yield return new WaitForSeconds(timeBetUpdateLetters);
        }

        IsTyping = false;
    }
}