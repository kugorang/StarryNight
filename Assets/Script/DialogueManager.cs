using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public TextDisplayer textDisplayer;
    public Image bgImg;
    //public GameObject alarmPanel;
    private DataController dataController;
    private DataDictionary dataDictionary;
    private Dictionary<int, TextInfo> dialogueDic;

    private static DialogueManager instance;

    public static DialogueManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += (scene, mode) => OnSceneChange(mode);

        dataController = DataController.Instance;
        dataDictionary = DataDictionary.Instance;
        dialogueDic = dataDictionary.DialogueDic;

        if (!dataController.IsTutorialEnd)
        {
            ShowDialogue();
        }
        else
        {
            textDisplayer.gameObject.SetActive(false);
        }
    }

    public void OnSceneChange(LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single)
        {
            if (GameObject.Find("Dialogue Panel") == null)
            {
                Debug.Log("Dialogue Panel is null object. Ignore this if this scene is not used in tutorial.");
                return;
            }
            Transform dialoguePanel = GameObject.Find("Dialogue Panel").transform;
            
            textDisplayer = dialoguePanel.Find("Text Displayer").GetComponent<TextDisplayer>();
            bgImg = textDisplayer.transform.Find("Holder").GetComponent<Image>();
            textDisplayer.gameObject.GetComponent<Button>().onClick.AddListener(this.OnClick);
            if (dataController.IsTutorialEnd)
            {
                textDisplayer.gameObject.SetActive(false);
                
            }
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