using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextDisplayer textDisplayer;
    public Image bgImg;
    //public GameObject alarmPanel;
    private DataController dataController;
    private DataDictionary dataDictionary;
    private Dictionary<int, TextInfo> dialogueDic;

    //private static DialogueManager instance;

    //public static DialogueManager Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = FindObjectOfType<DialogueManager>();

    //            if (instance == null)
    //            {
    //                GameObject container = new GameObject("DialogueManager");
    //                instance = container.AddComponent<DialogueManager>();
    //            }
    //        }

    //        return instance;
    //    }
    //}

    private void Awake()
    {
        //DontDestroyOnLoad(this);
        dataController = DataController.Instance;
        dataDictionary = DataDictionary.Instance;
        dialogueDic = dataDictionary.DialogueDic;

        if(dataController.IsTutorialEnd == 0)
        {
            ShowDialogue();
        }
        else
        {
            textDisplayer.gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        if (textDisplayer.IsTyping)
        {
            textDisplayer.SkipTypingLetter();
            return;
        }

        dataController.NowIndex++;
        
        if (dataController.NowIndex == 0)
        {
            textDisplayer.HideDialogueHolder();
            return;
        }

        TextInfo textInfo = dialogueDic[dataController.NowIndex];

        switch (textInfo.name)
        {
            case "메테스":
                bgImg.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("dialogueImg/metes/default-right");
                break;
            case "민":
                bgImg.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("dialogueImg/min/default-right");
                break;
            case "피터":
                bgImg.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("dialogueImg/peter/default-right");
                break;
            case "나레이션":
                bgImg.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("dialogueImg/none");
                break;
            case "알림창":
                textDisplayer.gameObject.SetActive(false);
                PopUpWindow.ShowDialogue(textInfo.dialogue);
                return;
            default:
                //textDisplayer.gameObject.SetActive(false);
                break;
        }

        textDisplayer.SetSay(textInfo.dialogue);
    }

    public void ContinueDialogue()
    {
        PopUpWindow.HideDialogue();
        textDisplayer.gameObject.SetActive(true);

        dataController.NowIndex++;
        ShowDialogue();
    }    

    private void ShowDialogue()
    {
        TextInfo textInfo = dialogueDic[dataController.NowIndex];

        if (textInfo.name == "알림창")
        {
            textDisplayer.gameObject.SetActive(false);
            PopUpWindow.ShowDialogue(textInfo.dialogue);
        }
        else
        {
            textDisplayer.SetSay(textInfo.dialogue);
        }
    }
}