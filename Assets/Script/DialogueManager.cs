using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script
{
    public class DialogueManager : MonoBehaviour, IEventListener
    {
        public Image BgImg;

        // 달성 체크용 상태 변수
        private int _condition300213;
        private int _condition300310;

        //public GameObject alarmPanel;
        private DataController _dataController;
        private DataDictionary _dataDictionary;
        private Dictionary<int, TextInfo> _dialogueDic;
        public TextDisplayer TextDisplayer;

        public static DialogueManager Instance { get; private set; }

        private void Awake()
        {
            _dataController = DataController.Instance;
            _dataDictionary = DataDictionary.Instance;
            _dialogueDic = _dataDictionary.DialogueDic;

            if (Instance == null)
            {
                Instance = this;
                
                // 등록 안 되어있으면 관찰자로 등록
                if (!_dataController.IsTutorialEnd && !_dataController.Observers.Contains(gameObject)) 
                    _dataController.Observers.Add(gameObject);
                
                DontDestroyOnLoad(gameObject);
            }

            SceneManager.sceneLoaded += OnSceneChange;

            if (!_dataController.IsTutorialEnd)
            {
                ShowDialogue();
            }
            else
            {
                TextDisplayer.gameObject.SetActive(false);
                
                // 튜토리얼 끝나면 구독 해제
                if (_dataController.Observers.Contains(gameObject))
                    _dataController.Observers.Remove(gameObject);
            }
        }

        private void OnSceneChange(Scene scene, LoadSceneMode mode)
        {
            // 무한 루프 방지용
            if (mode == LoadSceneMode.Single) 
            {   
                // 다이얼로그가 로드되지 않았으면 로드. count는 일반 씬 + Dialog인 경우만 정상으로 규정
                if (SceneManager.GetSceneByName("Dialog").isLoaded || SceneManager.sceneCount >= 2) 
                    return;
            
               // Debug.Log(scene.name + ", Count: " + SceneManager.sceneCount + " Active: " + SceneManager.GetActiveScene().name);
            
                /*var str = "";
            
                for (var i = 0; i < SceneManager.sceneCount; i++)
                    str += i + ":" + SceneManager.GetSceneAt(i).name + '\n';
            
                Debug.Log(str);*/
            
                SceneManager.LoadScene("Dialog", LoadSceneMode.Additive);
            }
            else if (mode == LoadSceneMode.Additive && scene == SceneManager.GetSceneByName("Dialog"))
            {
                if (GameObject.Find("Dialogue Panel") == null)
                {
                    Debug.LogWarning("Dialogue Panel is null object. Ignore this if this scene is not used in tutorial.");
                    return;
                }

                // 활성화되지 않은 오브젝트에 접근하려면 부모 오브젝트가 필요.
                var dialoguePanel = GameObject.Find("Dialogue Panel").transform; 

                TextDisplayer = dialoguePanel.Find("Text Displayer").GetComponent<TextDisplayer>();
                BgImg = TextDisplayer.transform.Find("Holder").GetComponent<Image>();
                /*TextDisplayer.gameObject.GetComponent<Button>().onClick.AddListener(OnClick);*/
                
                // 튜토리얼 끝나면 구독 해제
                if (!_dataController.IsTutorialEnd) 
                    return;
            
                TextDisplayer.gameObject.SetActive(false);
            }
        }

        private void OnClick()
        {
            if (TextDisplayer.IsTyping)
            {
                TextDisplayer.SkipTypingLetter();
                return;
            }

            // 현재 알림창이 떠 있다면, ContinueDialogue 이외의 방법으로 다음 대사로 넘어가선 안 됨.
            if (_dialogueDic[_dataController.NowIndex].Name != "알림창") 
                _dataController.NowIndex++;

            if (_dataController.NowIndex == 0)
            {
                /*TextDisplayer.HideDialogueHolder();*/
                TextDisplayer.gameObject.SetActive(false);
                return;
            }

            var textInfo = _dialogueDic[_dataController.NowIndex];

            switch (textInfo.Name)
            {
                case "메테스":
                    BgImg.gameObject.GetComponent<Image>().sprite =
                        Resources.Load<Sprite>("dialogueImg/metes/default-right");
                    break;
                case "민":
                    BgImg.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("dialogueImg/min/default-right");
                    break;
                case "피터":
                    BgImg.gameObject.GetComponent<Image>().sprite =
                        Resources.Load<Sprite>("dialogueImg/peter/default-right");
                    break;
                case "나레이션":
                    BgImg.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("dialogueImg/none");
                    break;
                case "알림창":
                    TextDisplayer.gameObject.SetActive(false);
                    PopUpWindow.ShowDialogue(textInfo.Dialogue);
                    return;
            }

            TextDisplayer.SetSay(textInfo.Dialogue);
        }

        private void ContinueDialogue()
        {
            PopUpWindow.HideDialogue();
            TextDisplayer.gameObject.SetActive(true);

            _dataController.NowIndex++;
            ShowDialogue();
        }

        public void ShowDialogue()
        {
            var textInfo = _dialogueDic[_dataController.NowIndex];

            if (textInfo.Name == "알림창")
            {
                TextDisplayer.gameObject.SetActive(false);
                PopUpWindow.ShowDialogue(textInfo.Dialogue);
            }
            else
            {
                if (TextDisplayer.gameObject.activeSelf == false)
                {
                    TextDisplayer.gameObject.SetActive(true);
                }
                
                TextDisplayer.SetSay(textInfo.Dialogue);
            }
        }

        #region OnEvent

        /// <inheritdoc />
        /// <summary>
        ///     클릭 시 호출
        /// </summary>
        /// <typeparam name="T">OnClick을 가진 컴포넌트</typeparam>
        /// <param name="obj">클릭된 오브젝트</param>
        /// <param name="options">0:버튼 정보(id 등) 1:씬인덱스(build Settings) id</param>
        public void OnObjClick<T>(T obj, params int[] options)
        {
            if (_dataController.IsTutorialEnd)
            {
                Debug.Log("tutorial is already finished");
                return;
            }

            // Type명으로 확인
            switch (obj.GetType().ToString()) 
            {
                case "Script.BlinkStar":
                    // 퀘스트
                    if (_dataController.NowIndex == 300135) 
                        ContinueDialogue();
                    break;
                case "Script.BookListManager":
                    if (_dataController.NowIndex == 300612 || _dataController.NowIndex == 300623) 
                        ContinueDialogue();
                    break;
                case "Script.CameraController":
                    var isLeft = options[0] > 0;
                
                    if (isLeft)
                    {
                        if (_dataController.NowIndex == 300210 || _dataController.NowIndex == 300617) 
                            ContinueDialogue();
                    }
                    else
                    {
                        if (_dataController.NowIndex == 300215) 
                            ContinueDialogue();
                    }
                    break;
                case "Script.CreateItem":
                    if (_dataController.NowIndex == 300129) 
                        ContinueDialogue();
                    break;
                case "Script.DataController":
                    break;
                case "Script.ItemInfo":
                    var index = options[0];
                
                    if (_dataController.NowIndex == 300305 && (index == 1001 || index == 1006 || index == 1011)) 
                        ContinueDialogue();
                    break;
                case "Script.ItemTimer":
                    if (_dataController.NowIndex == 300619) 
                        ContinueDialogue();
                    break;
                case "Script.SwitchSunMoon":
                    if (_dataController.NowIndex == 300212) 
                        ContinueDialogue();
                    break;
                case "Script.TmpButton":
                    if (_dataController.NowIndex == 300421 || _dataController.NowIndex == 300509) 
                        ContinueDialogue();
                    break;
                case "Script.UpgradeManager":
                    var upgradeId = options[0];
                
                    if (upgradeId == 0 && _dataController.NowIndex == 300515) 
                        ContinueDialogue();
                    break;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     슬라이드 시 호출
        /// </summary>
        /// <param name="isLeft">방향이 왼쪽인지 여부</param>
        /// <param name="sceneIndex">Build Settings의 씬 순서</param>
        public void OnSlide(bool isLeft, int sceneIndex)
        {
            if (_dataController.IsTutorialEnd)
            {
                Debug.Log("tutorial is already finished");
                return;
            }

            if (isLeft)
            {
                if (_dataController.NowIndex == 300617) 
                    ContinueDialogue();
            }
            else
            {
                if (_dataController.NowIndex == 300131) 
                    ContinueDialogue();
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     아이템 획득 시 호출
        /// </summary>
        /// <param name="item">얻은 아이템 정보</param>
        public void OnObtain(ItemInfo item)
        {
            if (_dataController.IsTutorialEnd)
            {
                Debug.Log("tutorial is already finished");
                return;
            }

            if (_dataController.NowIndex != 300213) 
                return;
        
            _condition300213++;

            if (_condition300213 >= 3) 
                ContinueDialogue();

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

        /// <inheritdoc />
        /// <summary>
        ///     아이템 판매 시 호출
        /// </summary>
        /// <param name="item">판매한 아이템 정보</param>
        public void OnSell(ItemInfo item)
        {
            if (_dataController.IsTutorialEnd)
            {
                Debug.Log("tutorial is already finished");
                return;
            }

            if (_dataController.NowIndex == 300422)
                ContinueDialogue();
        }

        /// <inheritdoc />
        /// <summary>
        ///     아이템 조합 시 호출
        /// </summary>
        /// <param name="itemA">터치한 아이템</param>
        /// <param name="itemB">닿은 아이템</param>
        /// <param name="result">결과물</param>
        public void OnCombine(ItemInfo itemA, ItemInfo itemB, ItemInfo result)
        {
            if (_dataController.IsTutorialEnd)
            {
                Debug.Log("tutorial is already finished");
                return;
            }

            if (_dataController.NowIndex == 300306)
            {
                ContinueDialogue();
            }
            else if (_dataController.NowIndex == 300310)
            {
                if (result.Index != 1002 && result.Index != 1007 && result.Index != 1012)
                    return;
                
                if (++_condition300310 >= 2)
                    ContinueDialogue();
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     값이 변화한 후 그것을 처리합니다.
        /// </summary>
        /// <param name="vt">변화한 값의 종류</param>
        /// <param name="args">0:변화 후 현재값 1:변화량</param>
        public void OnChangeValue(ValueType vt, int[] args)
        {
            if (_dataController.IsTutorialEnd)
            {
                Debug.Log("tutorial is already finished");
                return;
            }

            var currentValue = args[0];
            /*var delta = args[1];*/
        
            switch (vt)
            {
                case ValueType.Gold:
                    if (_dataController.NowIndex == 300427 && currentValue >= 200) 
                        ContinueDialogue();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("vt", vt, null);
            }
        }

        /// <inheritdoc />
        public void OnChangeValue(ValueType vt, float[] args)
        {
            if (_dataController.IsTutorialEnd)
            {
                Debug.Log("tutorial is already finished");
                return;
            }

            var currentValue = args[0];
            /*var delta = args[1];*/

            switch (vt)
            {
                case ValueType.Gold:
                    if (_dataController.NowIndex == 300426 && currentValue >= 200) 
                        ContinueDialogue();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("vt", vt, null);
            }
        }

        #endregion
    }
}