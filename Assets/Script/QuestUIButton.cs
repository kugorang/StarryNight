using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class QuestUIButton : MonoBehaviour
    {
        // TODO: Quest.cs 만든 후에는 두 버튼 관련 코드는 버튼 또는 UI매니저로 옮길 것.
        public GameObject PrevButton;
        public GameObject NextButton;
        private const int FirstQuest = 90101;
        private const int LastQuest = 90123; // 원래는 90247

        private static int _showingQuestIndex;
        private static int CurrentScene
        {
            get { return ( ShowingQuestIndex/ 100) % 10; }//1~12
        }

        private static readonly Dictionary<int, string> CurrentSceneName = new Dictionary<int, string>()
        {
            {0,"Aries"},
            {1,"Taurus"}
        };
        
        public static int ShowingQuestIndex
        {
            get { return _showingQuestIndex; }
            set
            {
                if (value < FirstQuest)
                    _showingQuestIndex = FirstQuest;
                else if (value > LastQuest)
                    _showingQuestIndex = LastQuest;
                else
                    _showingQuestIndex = value;
            }
        }

        public void Start()
        {
            // ShowingQuestIndex 값이 할당되지 않은 경우
            if (ShowingQuestIndex < FirstQuest) 
                ShowingQuestIndex = Quest.Progress;
        }

        // 슬라이드 기능 구현
        private void Update() 
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log(startPosX);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                /*var posXGap = Input.mousePosition.x - startPosX;*/

                //        if (Math.Abs(posXGap) > minimumDiff)
                //        {
                //            // ->
                //            if (posXGap > 0)
                //            {
                //                OnLeftQuestBtnClick();
                //            }
                //            // <-
                //            else if (posXGap < 0)
                //            {
                //                OnRightQuestBtnClick();
                //            }
                //        }
            }

            // 두 버튼의 활성화 담당. 첫 별자리일 때 참이 되는 조건이다.
            if (CurrentScene < 1)
            {
                // Taurus를 못 연 경우 두 버튼 다 끄기
                if (Quest.Progress < (int)Quest.Zodiac.Taurus) 
                {
                    PrevButton.SetActive(false);
                    NextButton.SetActive(false);
                }
                else
                {
                    PrevButton.SetActive(false);
                    NextButton.SetActive(true);
                }
            }
            // (_firstQuestsOf.Count - 1)은 _firstQuestsOf의 index중 가장 큰 값이다.
            // 따라서 마지막 별자리일 때 참이되는 조건이다.
            else if (CurrentScene >= Quest.NumberOfZodiac) 
            {
                PrevButton.SetActive(true);
                NextButton.SetActive(false);
            }
            else
            {
                PrevButton.SetActive(true);
                NextButton.SetActive(true);
            }
        }

        private void OnEnable()
        {
            var star = GameObject.Find(SceneManager.GetActiveScene().name + "_" + ShowingQuestIndex);
            
            if (star != null) 
                star.GetComponent<BlinkStar>().OnClick();
        }

        // TODO: Quest.cs 이후, 화면 전환 후 ShowQuestInfo 해주기 위해 쓰일 것 (n번째 별자리에서 누르면 n-1번째 별자리의 마지막 퀘스트를 보여줌)
        private void OnLeftQuestBtnClick()
        {
            /*
                ShowingQuestIndex -= 1;
                Debug.Log(ShowingQuestIndex + ", Left");
            */

            var star = GameObject.Find(CurrentSceneName[CurrentScene] + "_" + ShowingQuestIndex);
            
            if (star != null) 
                star.GetComponent<BlinkStar>().ShowQuestInfo();
        }

        // TODO: Quest.cs 이후,화면 전환 후 ShowQuestInfo 해주기 위해 쓰일 것 (n번째 별자리에서 누르면 n+1번째 별자리의 첫 퀘스트를 보여줌)
        private void OnRightQuestBtnClick()
        {
            var star = GameObject.Find(CurrentSceneName[CurrentScene] + "_" + ShowingQuestIndex);
            
            /*
                ShowingQuestIndex += 1;
            
                Debug.Log(ShowingQuestIndex + ", Right");
                
                if (ShowingQuestIndex > DataController.Instance.QuestProcess) //QuestProcess = 진행중인 퀘스트
                {
                    star = GameObject.Find(CurrentSceneName[CurrentScene] + "_" + ShowingQuestIndex);
                    
                    if (star != null)
                        star.GetComponent<BlinkStar>().OnClick();
                    
                    ShowingQuestIndex = DataController.Instance.QuestProcess;
                }
            */

            if (star != null)
                star.GetComponent<BlinkStar>().ShowQuestInfo();
        }
    }
}