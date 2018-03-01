using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestUIButton : MonoBehaviour {

    private const int FIRST_QUEST = 90101;
    private const int LAST_QUEST = 90123;//원래는 90247

    private float startPosX;
    private float minimumDiff;

    private static int showingQuestIndex;
    public static int ShowingQuestIndex
    {
        get
        {
            return showingQuestIndex;
        }
        set
        {
            if (value < FIRST_QUEST)
            {
                showingQuestIndex = FIRST_QUEST;
            }
            else if (value > LAST_QUEST)
            {
                showingQuestIndex = LAST_QUEST;
            }
            else
            {
                showingQuestIndex = value;
            }
        }
    }

    private List<int> FirstQuestsOf;
   // private Dictionary<int, string> CurrentSceneName;
    
    public void Start()
    {
        if (ShowingQuestIndex<FIRST_QUEST)//값이 할당되지 않은 경우
        {
            ShowingQuestIndex = DataController.Instance.QuestProcess;
        }
        FirstQuestsOf = DataDictionary.Instance.FirstQuestsOfScene;

        minimumDiff= Screen.width / 8;
    }

    private void Update()//주석풀어서확인, 슬라이드 구현 ppt 12번 참조
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosX = Input.mousePosition.x;
            //Debug.Log(startPosX);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            float posXGap = Input.mousePosition.x - startPosX;

            if (Math.Abs(posXGap) > minimumDiff)
            {
                // ->
                if (posXGap > 0)
                {
                    OnLeftQuestBtnClick();
                }
                // <-
                else if (posXGap < 0)
                {
                    OnRightQuestBtnClick();
                }
            }
        }
    }

    public void OnEnable()
    {

        GameObject star = GameObject.Find(SceneManager.GetActiveScene().name + "_" + ShowingQuestIndex);
        if (star != null)
        {
            star.GetComponent<BlinkStar>().OnClick();
        }
    }
    
    public void OnLeftQuestBtnClick()
    {
        ShowingQuestIndex -= 1;
        Debug.Log(ShowingQuestIndex + ", Left");
        if (ShowingQuestIndex < FirstQuestsOf[1] && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Aries"))
        {
            AudioManager.GetInstance().ActSound();
            SceneManager.LoadScene("Aries");
            return;
        }

        GameObject star = GameObject.Find(SceneManager.GetActiveScene().name + "_" + ShowingQuestIndex);
        if (star != null)
        {
            star.GetComponent<BlinkStar>().ShowQuestInfo();
        }

    }

    public void OnRightQuestBtnClick()
    {
        GameObject star;
        ShowingQuestIndex += 1;
        Debug.Log(ShowingQuestIndex + ", Right");
        if (ShowingQuestIndex > DataController.Instance.QuestProcess)//progress이 진행중 퀘스트
        {
            star = GameObject.Find(SceneManager.GetActiveScene().name + "_" + ShowingQuestIndex);
            if (star != null)
            {
                star.GetComponent<BlinkStar>().OnClick();

            }
            ShowingQuestIndex = DataController.Instance.QuestProcess;
        }
        if (FirstQuestsOf[1] <= ShowingQuestIndex && ShowingQuestIndex < FirstQuestsOf[2])
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Taurus"))
            {
                AudioManager.GetInstance().ActSound();
                SceneManager.LoadScene("Taurus");
                return;
            }
        }
        /* else if(ShowingQuestIndex > 90123)
         {
             SceneManager.LoadScene("Quest");
         }*/
        star = GameObject.Find(SceneManager.GetActiveScene().name + "_" + ShowingQuestIndex);
        if (star != null)
        {
            star.GetComponent<BlinkStar>().ShowQuestInfo();
        }
    }
}
