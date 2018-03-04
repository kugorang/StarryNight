using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour, IEventListener
{
    public TextDisplayer textDisplayer;
    public Image bgImg;
    //public GameObject alarmPanel;
    private DataController dataController;
    private DataDictionary dataDictionary;
    private Dictionary<int, TextInfo> dialogueDic;

    //달성체크용 상태 변수
    int condition300213 = 0;
    int condition300310 = 0;
 

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
        dataController = DataController.Instance;
        dataDictionary = DataDictionary.Instance;
        dialogueDic = dataDictionary.DialogueDic;


        if (instance == null)
        {
            instance = this;
            if (!dataController.IsTutorialEnd&&!dataController.Observers.Contains(gameObject))//등록 안 되어있으면 관찰자로 등록
            {
                dataController.Observers.Add(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += (scene, mode) => OnSceneChange(scene, mode);


        if (!dataController.IsTutorialEnd)
        {
            ShowDialogue();
        }
        else
        {
            textDisplayer.gameObject.SetActive(false);
            if (dataController.Observers.Contains(gameObject))//튜토리얼 끝나면 구독해제
            {
                dataController.Observers.Remove(gameObject);
            }
        }
    }

    public void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single)//무한 루프 방지용
        {
          if (!SceneManager.GetSceneByName("Dialog").isLoaded&&SceneManager.sceneCount<2)//다이얼로그가 로드 되지 않았으면 로드. count는 일반 씬+Dialog인 경우만 정상으로 규정
            {
                Debug.Log(scene.name+", Count: "+SceneManager.sceneCount+" Active: "+SceneManager.GetActiveScene().name);
                string str="";
                for(var i=0; i < SceneManager.sceneCount; i++)
                {
                    str+=i + ":" + SceneManager.GetSceneAt(i).name+'\n';
                }
                Debug.Log(str);
                SceneManager.LoadScene("Dialog",LoadSceneMode.Additive);
          }
            
        }  
        else if (mode==LoadSceneMode.Additive&&scene==SceneManager.GetSceneByName("Dialog"))
        {
            if (GameObject.Find("Dialogue Panel") == null)
            {
                Debug.LogWarning("Dialogue Panel is null object. Ignore this if this scene is not used in tutorial.");
                return;
            }

            Transform dialoguePanel = GameObject.Find("Dialogue Panel").transform;//활성화되지 않은 오브젝트에 접근하려면 부모 오브젝트가 필요.

            textDisplayer = dialoguePanel.Find("Text Displayer").GetComponent<TextDisplayer>();
            bgImg = textDisplayer.transform.Find("Holder").GetComponent<Image>();
            textDisplayer.gameObject.GetComponent<Button>().onClick.AddListener(this.OnClick);


            if (dataController.IsTutorialEnd)//튜토리얼 끝나면 구독해제
            {
                textDisplayer.gameObject.SetActive(false);
                if (dataController.Observers.Contains(gameObject))
                {
                    dataController.Observers.Remove(gameObject);
                }
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

        if (dialogueDic[
            dataController
            .NowIndex]
            .name!="알림창")//현재 알림창이 떠 있다면, ContinueDialogue 이외의 방법으로 다음 대사로 넘어가선 안 됨.
        {
            dataController.NowIndex++;
        }
        
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

    #region OnEvent
    /// <summary>
    /// 클릭 시 호출
    /// </summary>
    /// <typeparam name="T">OnClick을 가진 컴포넌트</typeparam>
    /// <param name="obj">클릭된 오브젝트</param>
    /// <param name="options">0:버튼 정보(id 등) 1:씬인덱스(build Settings) id</param>
    public void OnObjClick<T>(T obj, params int[] options)
    {
        if (dataController.IsTutorialEnd)
        {
            Debug.Log("tutorial is already finished");
            return;
        }
        switch (obj.GetType().ToString())//Type명으로 확인
        {
            case "BlinkStar":
                if ( dataController.NowIndex == 300135)//퀘스트
                {
                    ContinueDialogue();
                }
                break;
            case "BookListManager":
                if (dataController.NowIndex == 300612 || dataController.NowIndex == 300623)
                {
                    ContinueDialogue();
                }
                break;
            case "CameraController":
                bool isLeft = options[0] > 0;
                if (isLeft)
                {
                    if (dataController.NowIndex == 300210 || dataController.NowIndex == 300617)
                    {
                        ContinueDialogue();
                    }
                }
                else
                {
                    if (dataController.NowIndex == 300215)
                    {
                        ContinueDialogue();
                    }
                }
                break;
            case "CreateItem":
                if (dataController.NowIndex == 300129)
                {
                    ContinueDialogue();
                }
                break;
            case "DataController":
                break;
            case "ItemInfo":
                int index = options[0];
                if (dataController.NowIndex == 300305 && index >= 1001 && index <= 1003)
                {
                    ContinueDialogue();
                }
                break;
            case "ItemTimer":
                if (dataController.NowIndex == 300619)
                {
                    ContinueDialogue();
                }
                break;
            case "SwitchSunMoon":
                if(dataController.NowIndex == 300212)
                {
                    ContinueDialogue();
                }
                break;
            case "TmpButton":
                if (dataController.NowIndex == 300421 || dataController.NowIndex == 300509)
                {
                   ContinueDialogue();
                }
                break;
            case "UpgradeManager":
                int upgradeId = options[0];
                if (upgradeId == 0 && dataController.NowIndex == 300515)
                {
                    ContinueDialogue();
                }
                break;            
            default:
                break;
        }

        
    }
    /// <summary>
    /// 슬라이드 시 호출
    /// </summary>
    /// <param name="isLeft">방향이 왼쪽인지 여부</param>
    /// <param name="sceneIndex">Build Settings의 씬 순서</param>
    public void OnSlide(bool isLeft, int sceneIndex)
    {
        if (dataController.IsTutorialEnd)
        {
            Debug.Log("tutorial is already finished");
            return;
        }
        if (isLeft)
        {
            if (dataController.NowIndex == 300617)
            {
                ContinueDialogue();
            }
        }
        else
        {
            if (dataController.NowIndex == 300131)
            {
                ContinueDialogue();
            }
        }
    }
    /// <summary>
    /// 아이템 획득 시 호출
    /// </summary>
    /// <param name="item">얻은 아이템 정보</param>
    public void OnObtain(ItemInfo item)
    {
        if (dataController.IsTutorialEnd)
        {
            Debug.Log("tutorial is already finished");
            return;
        }

        if (dataController.NowIndex == 300213 )
        {
            
                condition300213++;
            
            if (condition300213 >= 3)
            {
                ContinueDialogue();
            }
            
        }

        /*if (dataController.NowIndex == 300310)
        {
            
            롤백을 대비해 기존코드 임시로 남겨 둠
              int sum = 0;

            foreach (KeyValuePair<int, Dictionary<int, SerializableVector3>> entry in dataController.HaveDic)
            {
                switch (entry.Key)
                {
                    case 1004:
                    case 1005:
                    case 1006:
                        sum += entry.Value.Count;
                        break;
                }
            }

            if (sum >= 2)
            {
                GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>().ContinueDialogue();
            }
        }*/
    }
    /// <summary>
    /// 아이템 판매 시 호출
    /// </summary>
    /// <param name="item">판매한 아이템 정보</param>
    public void OnSell(ItemInfo item)
    {
        if (dataController.IsTutorialEnd)
        {
            Debug.Log("tutorial is already finished");
            return;
        }
        if (dataController.NowIndex == 300423)
        {
            ContinueDialogue();
        }
    }
    /// <summary>
    /// 아이템 조합 시 호출
    /// </summary>
    /// <param name="itemA">터치한 아이템</param>
    /// <param name="itemB">닿은 아이템</param>
    /// <param name="result">결과물</param>
    public void OnCombine(ItemInfo itemA, ItemInfo itemB, ItemInfo result)
    {
        if (dataController.IsTutorialEnd)
        {
            Debug.Log("tutorial is already finished");
            return;
        }
        if (dataController.NowIndex == 300306)
        {
            ContinueDialogue();
        }
        else if (dataController.NowIndex == 300310)
        {
            if (1003<result.Index&& result.Index < 1007)
            {
                condition300310++;
            }
            if (condition300310>=2)
            {
                ContinueDialogue();
            }
        }
    }
    /// <summary>
    /// 값이 변화한 후 그것을 처리합니다.
    /// </summary>
    /// <param name="vt">변화한 값의 종류</param>
    /// <param name="args">0:변화 후 현재값 1:변화량</param>
    public void OnChangeValue(ValueType vt, int[] args)
    {
        if (dataController.IsTutorialEnd)
        {
            Debug.Log("tutorial is already finished");
            return;
        }
        int currentValue = args[0];
        int delta = args[1];
        switch (vt)
        {
            case ValueType.Gold:
                if (dataController.NowIndex == 300427 && currentValue>=200)
                {
                    ContinueDialogue();
                }
                break;
        }       
    }
    public void OnChangeValue(ValueType vt, float[] args)
    {
        if (dataController.IsTutorialEnd)
        {
            Debug.Log("tutorial is already finished");
            return;
        }
        float currentValue = args[0];
        float delta = args[1];
        switch (vt)
        {
            case ValueType.Gold:
                if (dataController.NowIndex == 300427 && currentValue >= 200)
                {
                    ContinueDialogue();
                }
                break;
        }
    }
    #endregion
}