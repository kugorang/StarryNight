using System;
using System.Collections;
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
        private int _condition300310;

        //public GameObject alarmPanel;
        private DataController _dataController;
        private DataDictionary _dataDictionary;
        private Dictionary<int, TextInfo> _dialogueDic;
        public TextDisplayer TextDisplayer;
        public GameObject[] Fingers;    // 0이 편 손가락, 1이 굽힌 손가락
        private IEnumerator _fingerAnim;
        private bool _runningAnim;

        public static DialogueManager Instance { get; private set; }

        private void Awake()
        {
            _dataController = DataController.Instance;
            _dataDictionary = DataDictionary.Instance;
            _dialogueDic = _dataDictionary.DialogueDic;
            _runningAnim = false;

            if (Instance == null)
            {
                Instance = this;
                
                // 등록 안 되어있으면 관찰자로 등록
                if (!_dataController.IsTutorialEnd && !_dataController.Observers.Contains(gameObject)) 
                    _dataController.Observers.Add(gameObject);
                
                /*DontDestroyOnLoad(gameObject);*/
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

            /*_dataController.NowIndex = 300133;*/
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

        // TODO: 언젠가.. 하드 코딩 되어 있는 퀘스트 인덱스를 파일로 읽어와서 바꾸는 코드로 수정... 지금은 시간이 너무 없다
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

            var nowIndex = _dataController.NowIndex;
            
            switch (nowIndex)
            {
                case 0:
                    // NowIndex가 0을 반환한 경우는 다이얼로그를 끝내는 인덱스이므로 TextDisplayer를 비활성화한 후 메소드를 종료한다.
                    /*TextDisplayer.HideDialogueHolder(); // 이전에 사용했던 TextDisplayer 비활성화 코드*/
                    TextDisplayer.gameObject.SetActive(false);
                    return;
                // 아래는 손가락 이미지를 표시하기 위한 코드
                case 300130:    // 지구본을 클릭하는 튜토리얼
                case 300132:    // 오른쪽으로 슬라이드하는 튜토리얼
                case 300135:    // 퀘스트 버튼을 클릭하는 튜토리얼
                /*case 300136:    // 반짝이는 별을 클릭하는 튜토리얼
                case 300204:    // 뒤로가기 버튼을 클릭하는 튜토리얼
                case 300210:    // 왼쪽 화살표 버튼을 클릭하는 튜토리얼
                case 300212:    // 별 버튼을 클릭하는 튜토리얼
                case 300213:    // 재료 아이템 1개를 구하는 튜토리얼
                case 300215:    // 오른쪽 화살표 버튼을 클릭하는 튜토리얼
                case 300217:    // 퀘스트 버튼을 클릭하는 튜토리얼
                case 300305:    // 별 아이템을 클릭하는 튜토리얼
                case 300306:    // 조합을 하는 튜토리얼
                case 300310:    // 별의 원석 2개로 별의 파편을 1개 만드는 튜토리얼
                case 300422:    // 아이템을 끌어서 판매하는 튜토리얼
                case 300426:    // 200 골드를 획득하는 튜토리얼
                case 300514:    // 양털 가방을 업그레이드하는 튜토리얼
                case 300609:    // 서적 버튼을 클릭하는 튜토리얼
                case 300612:    // 느낌표가 붙은 서적 아이템을 클릭하는 튜토리얼
                case 300617:    // 지구본이 있는 화면으로 이동하는 튜토리얼
                case 300619:    // 왼쪽 화면 상단의 파란색 별을 클릭하는 튜토리얼
                case 300622:    // 서적 버튼을 클릭하는 튜토리얼
                case 300623:    // 활성화되지 않은 서적 재료 아이템을 클릭하는 튜토리얼*/
                    _fingerAnim = FingerAnim(nowIndex);
                    StartCoroutine(_fingerAnim);
                    break;
            }

            var textInfo = _dialogueDic[nowIndex];

            SwitchCharacterImage(textInfo);
            TextDisplayer.SetSay(textInfo.Dialogue);
        }

        private IEnumerator FingerAnim(int questIndex)
        {
            _runningAnim = true;
            
            foreach (var finger in Fingers)
            {
                finger.SetActive(true);
            }
            
            // 반복되지 않아야 하는 초기화 작업을 여기서 진행한다.
            switch (questIndex)
            {
                case 300130:    // 지구본을 클릭하는 튜토리얼
                    Fingers[1].transform.position = new Vector3(-456, -637, -3);
                    Fingers[1].SetActive(false);
                    break;
                case 300135:    // 퀘스트 버튼을 클릭하는 튜토리얼
                    Fingers[0].transform.position = new Vector3(465, -930, -3);
                    Fingers[1].transform.position = new Vector3(465, -930, -3);
                    /*Fingers[1].SetActive(false);*/
                    break;
                case 300136:    // 반짝이는 별을 클릭하는 튜토리얼
                    break;
                case 300204:    // 뒤로가기 버튼을 클릭하는 튜토리얼
                    break;
                case 300210:    // 왼쪽 화살표 버튼을 클릭하는 튜토리얼
                    break;
                case 300212:    // 별 버튼을 클릭하는 튜토리얼
                    break;
                case 300213:    // 재료 아이템 1개를 구하는 튜토리얼
                    break;
                case 300215:    // 오른쪽 화살표 버튼을 클릭하는 튜토리얼
                    break;
                case 300217:    // 퀘스트 버튼을 클릭하는 튜토리얼
                    break;
                case 300305:    // 별 아이템을 클릭하는 튜토리얼
                    break;
                case 300306:    // 조합을 하는 튜토리얼
                    break;
                case 300310:    // 별의 파편을 1개 만드는 튜토리얼
                    break;
                case 300422:    // 아이템을 끌어서 판매하는 튜토리얼
                    break;
                case 300426:    // 200 골드를 획득하는 튜토리얼
                    break;
                case 300514:    // 양털 가방을 업그레이드하는 튜토리얼
                    break;
                case 300609:    // 서적 버튼을 클릭하는 튜토리얼
                    break;
                case 300612:    // 느낌표가 붙은 서적 아이템을 클릭하는 튜토리얼
                    break;
                case 300617:    // 지구본이 있는 화면으로 이동하는 튜토리얼
                    break;
                case 300619:    // 왼쪽 화면 상단의 파란색 별을 클릭하는 튜토리얼
                    break;
                case 300622:    // 서적 버튼을 클릭하는 튜토리얼
                    break;
                case 300623:    // 활성화되지 않은 서적 재료 아이템을 클릭하는 튜토리얼
                    break;    
            }
            
            while (_dataController.NowIndex == questIndex)
            {
                switch (questIndex)
                {
                    case 300130:    // 지구본을 클릭하는 튜토리얼
                        // 위치 설정
                        Fingers[0].transform.position = new Vector3(-356, -737, -3);
                        
                        // 이동하기
                        iTween.MoveBy(Fingers[0], iTween.Hash("x", -100, "y", 100, "time", 0.8f));
                        
                        yield return new WaitForSeconds(0.8f);
                
                        // 구부리기
                        Fingers[0].SetActive(false);
                        Fingers[1].SetActive(true);
                
                        yield return new WaitForSeconds(0.4f);
                
                        // 펴기
                        Fingers[0].SetActive(true);
                        Fingers[1].SetActive(false);
                
                        yield return new WaitForSeconds(0.4f);
                        break;
                    case 300132:    // 오른쪽으로 슬라이드하는 튜토리얼
                        // 위치 설정
                        Fingers[0].transform.position = new Vector3(-230, -600, -3);
                        Fingers[1].transform.position = new Vector3(-230, -600, -3);
                        
                        // 펴기
                        Fingers[0].SetActive(true);
                        Fingers[1].SetActive(false);
                
                        yield return new WaitForSeconds(0.4f);
                        
                        // 구부리기
                        Fingers[0].SetActive(false);
                        Fingers[1].SetActive(true);
                
                        yield return new WaitForSeconds(0.4f);
                        
                        // 이동하기
                        iTween.MoveBy(Fingers[1], iTween.Hash("x", -370, "y", 0, "time", 0.8f));
                        Fingers[0].transform.position = new Vector3(-600, -600, -3);
                        
                        yield return new WaitForSeconds(0.8f);
                
                        // 펴기
                        Fingers[0].SetActive(true);
                        Fingers[1].SetActive(false);
                
                        yield return new WaitForSeconds(0.4f);
                        break;
                    case 300135:    // 퀘스트 버튼을 클릭하는 튜토리얼
                        /*// 위치 설정
                        Fingers[0].transform.position = new Vector3(500, -700, -3);
                        
                        // 이동하기
                        iTween.MoveBy(Fingers[0], iTween.Hash("x", -35, "y", -230, "time", 1.5f));
                        
                        yield return new WaitForSeconds(1.5f);
                
                        // 구부리기
                        Fingers[0].SetActive(false);
                        Fingers[1].SetActive(true);
                
                        yield return new WaitForSeconds(0.4f);
                
                        // 펴기
                        Fingers[0].SetActive(true);
                        Fingers[1].SetActive(false);
                
                        yield return new WaitForSeconds(0.4f);*/
                        
                        // 펴기
                        Fingers[0].SetActive(true);
                        Fingers[1].SetActive(false);
                
                        yield return new WaitForSeconds(0.4f);
                        
                        // 구부리기
                        // 씬이 넘어가면 MissingReferenceException이 뜨기 때문에 현재 씬이 Main인지를 확인한다.
                        if (SceneManager.GetActiveScene().name == "Main")
                        {
                            Fingers[0].SetActive(false);
                            Fingers[1].SetActive(true);    
                        }
                
                        yield return new WaitForSeconds(0.4f);
                        break;
                    case 300136:    // 반짝이는 별을 클릭하는 튜토리얼
                        break;
                    case 300204:    // 뒤로가기 버튼을 클릭하는 튜토리얼
                        break;
                    case 300210:    // 왼쪽 화살표 버튼을 클릭하는 튜토리얼
                        break;
                    case 300212:    // 별 버튼을 클릭하는 튜토리얼
                        break;
                    case 300213:    // 재료 아이템 1개를 구하는 튜토리얼
                        break;
                    case 300215:    // 오른쪽 화살표 버튼을 클릭하는 튜토리얼
                        break;
                    case 300217:    // 퀘스트 버튼을 클릭하는 튜토리얼
                        break;
                    case 300305:    // 별 아이템을 클릭하는 튜토리얼
                        break;
                    case 300306:    // 조합을 하는 튜토리얼
                        break;
                    case 300310:    // 별의 파편을 1개 만드는 튜토리얼
                        break;
                    case 300422:    // 아이템을 끌어서 판매하는 튜토리얼
                        break;
                    case 300426:    // 200 골드를 획득하는 튜토리얼
                        break;
                    case 300514:    // 양털 가방을 업그레이드하는 튜토리얼
                        break;
                    case 300609:    // 서적 버튼을 클릭하는 튜토리얼
                        break;
                    case 300612:    // 느낌표가 붙은 서적 아이템을 클릭하는 튜토리얼
                        break;
                    case 300617:    // 지구본이 있는 화면으로 이동하는 튜토리얼
                        break;
                    case 300619:    // 왼쪽 화면 상단의 파란색 별을 클릭하는 튜토리얼
                        break;
                    case 300622:    // 서적 버튼을 클릭하는 튜토리얼
                        break;
                    case 300623:    // 활성화되지 않은 서적 재료 아이템을 클릭하는 튜토리얼
                        break;    
                }
            }

            FinishFingerAnim();

            yield return null;
        }

        private void FinishFingerAnim()
        {
            foreach (var finger in Fingers)
            {
                if (finger == null || !finger.activeSelf) 
                    continue;
                
                iTween.Stop(finger);
                finger.SetActive(false);
            }

            _runningAnim = false;
        }

        private void ContinueDialogue()
        {
            if (_runningAnim && !_dataController.IsTutorialEnd)
            {
                StopCoroutine(_fingerAnim);
                FinishFingerAnim();
            }
            
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
                // 퀘스트를 눌렀을 때 다이얼로그를 띄우기 위한 코드
                if (TextDisplayer.gameObject.activeSelf == false)
                {
                    // 평소에 비활성화 되어 있는 TextDisplayer를 활성화 시킨 후 배경 이미지 변경
                    TextDisplayer.gameObject.SetActive(true);
                    SwitchCharacterImage(textInfo);
                }
                
                TextDisplayer.SetSay(textInfo.Dialogue);
            }
        }

        private void SwitchCharacterImage(TextInfo textInfo)
        {
            switch (textInfo.Name)
            {
                case "메테스":
                    switch (textInfo.Face)
                    {
                        case "놀람":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/metes/surprise-right");
                            break;
                        case "슬픔":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/metes/sad-right");
                            break;
                        case "웃음":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/metes/laugh-right");
                            break;
                        case "화남":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/metes/angry-right");
                            break;
                        default:    // 메인 혹은 기타
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/metes/default-right");
                            break;
                    }
                    break;
                case "민":    case "??? (민)":
                    switch (textInfo.Face)
                    {
                        case "놀람":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/min/surprise-right");
                            break;
                        case "슬픔":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/min/sad-right");
                            break;
                        case "웃음":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/min/laugh-right");
                            break;
                        case "화남":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/min/angry-right");
                            break;
                        default:    // 메인 혹은 기타
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/min/default-right");
                            break;
                    }
                    break;
                case "피터":
                    switch (textInfo.Face)
                    {
                        case "놀람":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/peter/surprise-right");
                            break;
                        case "슬픔":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/peter/sad-right");
                            break;
                        case "웃음":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/peter/laugh-right");
                            break;
                        case "화남":
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/peter/angry-right");
                            break;
                        default:    // 메인 혹은 기타
                            BgImg.gameObject.GetComponent<Image>().sprite =
                                Resources.Load<Sprite>("dialogueImg/peter/default-right");
                            break;
                    }
                    break;
                case "유노":    case "??? (유노)":
                    // TODO : 유노 이미지가 나오면 유노 이미지로 바꿀 것. 현재는 없는 상태
                    BgImg.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("dialogueImg/none");
                    break;
                case "아나":    case "??? (아나)":
                    // TODO : 아나 이미지가 나오면 아나 이미지로 바꿀 것. 현재는 없는 상태
                    BgImg.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("dialogueImg/none");
                    break;
                case "나레이션":
                    BgImg.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("dialogueImg/none");
                    break;
                case "알림창":
                    TextDisplayer.gameObject.SetActive(false);
                    PopUpWindow.ShowDialogue(textInfo.Dialogue);
                    return;
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
                    // 반짝이는 별을 클릭하는 퀘스트
                    if (_dataController.NowIndex == 300136) 
                        ContinueDialogue();
                    break;
                case "Script.BookListManager":
                    // 느낌표가 붙은 서적 아이템을 클릭하는 퀘스트
                    // 또는 활성화되지 않은 서적 재료 아이템을 클릭하는 퀘스트
                    if (_dataController.NowIndex == 300612 || _dataController.NowIndex == 300623)
                        ContinueDialogue();
                    break;
                case "Script.ButtonManager":
                    // 두 가지 모두 퀘스트 버튼을 클릭하는 퀘스트
                    if (_dataController.NowIndex == 300135 || _dataController.NowIndex == 300217)
                        ContinueDialogue();
                    break;
                case "Script.CameraController":
                    var isLeft = options[0] > 0;
                
                    if (isLeft)
                    {
                        // 왼쪽 화살표 버튼을 클릭하는 퀘스트
                        // 또는 지구본이 있는 화면으로 이동하는 퀘스트
                        if (_dataController.NowIndex == 300210 || _dataController.NowIndex == 300617) 
                            ContinueDialogue();
                    }
                    else
                    {
                        // 오른쪽 화살표 버튼을 클릭하는 퀘스트
                        if (_dataController.NowIndex == 300215) 
                            ContinueDialogue();
                    }
                    break;
                case "Script.CreateItem":
                    // 지구본을 클릭하는 퀘스트
                    if (_dataController.NowIndex == 300130)
                        ContinueDialogue();
                    break;
                /*case "Script.DataController":
                    break;*/
                case "Script.ItemInfo":
                    var index = options[0];
                
                    // 별의 원석을 클릭하는 퀘스트
                    if (_dataController.NowIndex == 300305 && (index == 1001 || index == 1006 || index == 1011)) 
                        ContinueDialogue();
                    break;
                case "Script.ItemTimer":
                    // 화면 상단의 파란색 별을 클릭하는 퀘스트
                    if (_dataController.NowIndex == 300619) 
                        ContinueDialogue();
                    break;
                case "Script.SwitchSunMoon":
                    // 별 버튼을 클릭하는 퀘스트
                    if (_dataController.NowIndex == 300212) 
                        ContinueDialogue();
                    break;
                case "Script.UpgradeManager":
                    var upgradeId = options[0];
                
                    // 양털 가방을 업그레이드 하는 퀘스트
                    if (upgradeId == 0 && _dataController.NowIndex == 300514) 
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
                // 지구본이 있는 화면으로 이동하는 퀘스트
                if (_dataController.NowIndex == 300617) 
                    ContinueDialogue();
            }
            else
            {
                // 슬라이드를 하여 오른쪽 화면으로 이동하는 퀘스트
                if (_dataController.NowIndex == 300132) 
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

            // 재료 아이템 1개를 구하는 퀘스트
            if (_dataController.NowIndex == 300213)
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

            // 아이템을 판매하는 퀘스트
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

            // 아이템을 조합하는 퀘스트
            if (_dataController.NowIndex == 300306)
            {
                ContinueDialogue();
            }
            // 별의 파편 1개를 만드는 퀘스트
            else if (_dataController.NowIndex == 300310
                     && (result.Index == 1002 || result.Index == 1007 || result.Index == 1012) )
                ContinueDialogue();
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
                    // 200 골드를 획득하는 퀘스트
                    if (_dataController.NowIndex == 300426 && currentValue >= 200) 
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
                    // 아이템을 판매하는 퀘스트
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