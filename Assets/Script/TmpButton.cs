using UnityEngine;

public class TmpButton : MonoBehaviour
{
    public void OnClick()
    {
        if (!DataController.Instance.IsTutorialEnd && (DataController.Instance.NowIndex == 300421 || DataController.Instance.NowIndex == 300509))
        {
            GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>().ContinueDialogue();
        }
    }
}