using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [Tooltip("지구본과 겹치지 않게 적절한 X를 주세요.")]
    public float minimumX;              // 210 이상, 지구본과 겹치치 않게.
    private float startPosX;
    private DataController dataController;
    public DialogueManager dialogueManager;
    private Scene nowScene;

    public int maxSceneNum;
    private int nowSceneNum;

    private int LastScene
    {
        get
        {
            if (nowScene.name == "Main")
            {
                return PlayerPrefs.GetInt("MainLastScene", 0);
            }

            return PlayerPrefs.GetInt("QuestLastScene", 0);
        }
        set
        {
            if (nowScene.name == "Main")
            {
                PlayerPrefs.SetInt("MainLastScene", value);
            }
            else
            {
                PlayerPrefs.SetInt("QuestLastScene", value);
            }
        }
    }

    private int minimumDiff;
    public static bool FocusOnItem { get; set; }

    private void Awake()
    {
        FocusOnItem = false;
        minimumDiff = Screen.width / 8;
        dataController = DataController.Instance;
        nowScene = SceneManager.GetActiveScene();
        nowSceneNum = LastScene;

        transform.position = new Vector3(transform.position.x + LastScene * 1080.0f, transform.position.y, transform.position.z);
    }

    public GameObject mainCamera;

    private void Update() // 주석 풀어서 확인, 슬라이드 구현 ppt 12번 참조
    {
        if (Input.GetMouseButtonDown(0) && !FocusOnItem)
        {
            startPosX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0) && !FocusOnItem)
        {
            float posXGap = Input.mousePosition.x - startPosX;

            if (Math.Abs(posXGap) > minimumDiff)
            {
                // 오른쪽에서 왼쪽 (<-)
                if (posXGap > 0 && nowSceneNum > 0)
                {
                    if (dataController.IsTutorialEnd == 0 && dataController.NowIndex == 300617)
                    {
                        dialogueManager.ContinueDialogue();
                    }

                    iTween.MoveTo(gameObject, iTween.Hash("x", transform.position.x - 1080.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));

                    LastScene = --nowSceneNum;
                }
                // 왼쪽에서 오른쪽 (->)
                else if (posXGap < 0 && startPosX > minimumX && nowSceneNum < maxSceneNum - 1)
                {
                    if (dataController.IsTutorialEnd == 0 && dataController.NowIndex == 300131)
                    {
                        dialogueManager.ContinueDialogue();
                    }
                    
                    iTween.MoveTo(gameObject, iTween.Hash("x", transform.position.x + 1080.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
                    LastScene = ++nowSceneNum;
                }
            }
        }
    }

    public void OnClickLeftBtn()
    {
        if (dataController.IsTutorialEnd == 0 && (dataController.NowIndex == 300210 || dataController.NowIndex == 300617))
        {
            dialogueManager.ContinueDialogue();
        }

        iTween.MoveTo(mainCamera, iTween.Hash("x", transform.position.x - 1080.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
        LastScene = --nowSceneNum;
    }

    public void OnClickRightBtn()
    {
        if (dataController.IsTutorialEnd == 0 && dataController.NowIndex == 300215)
        {
            dialogueManager.ContinueDialogue();
        }

        iTween.MoveTo(mainCamera, iTween.Hash("x", transform.position.x + 1080.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
        LastScene = ++nowSceneNum;
    }

    public void OnApplicationQuit()
    {
        LastScene = nowSceneNum;
    }
}