using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestUIButton : MonoBehaviour {

    private static int showingQuestIndex;
    public static int ShowingQuestIndex
    {
        get
        {
            return showingQuestIndex;
        }
        set
        {
            if (value < 90101)
            {
                showingQuestIndex = 90101;
            }
            else if (value > 90123)
            {
                showingQuestIndex = 90123;
            }
            else
            {
                showingQuestIndex = value;
            }
        }
    }

    public void Start()
    {
        if (ShowingQuestIndex<90101)//값이 할당되지 않은 경우
        {
            ShowingQuestIndex = DataController.GetInstance().QuestProcess;
        }
    }

    public void OnEnable()
    {
        Debug.Log(ShowingQuestIndex + ", OnEnable, " + SceneManager.GetActiveScene().name);
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
        if (ShowingQuestIndex <= 90104 && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Aris"))
        {
            AudioManager.GetInstance().ActSound();
            SceneManager.LoadScene("Aris");
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
        if (ShowingQuestIndex > DataController.GetInstance().QuestProcess)//progress이 진행중 퀘스트
        {
            star = GameObject.Find(SceneManager.GetActiveScene().name + "_" + ShowingQuestIndex);
            if (star != null)
            {
                star.GetComponent<BlinkStar>().OnClick();

            }
            ShowingQuestIndex = DataController.GetInstance().QuestProcess;
        }
        if (90104 < ShowingQuestIndex && ShowingQuestIndex <= 90123)
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
