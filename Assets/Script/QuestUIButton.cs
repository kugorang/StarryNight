using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class QuestUIButton : MonoBehaviour
    {
        private const int FirstQuest = 90101;
        private const int LastQuest = 90123; //원래는 90247

        private static int _showingQuestIndex;
        private Dictionary<int, string> _currentSceneName;

        private List<int> _firstQuestsOf;

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
            if (ShowingQuestIndex < FirstQuest) //값이 할당되지 않은 경우
                ShowingQuestIndex = DataController.Instance.QuestProcess;
            _firstQuestsOf = DataDictionary.Instance.FirstQuestsOfScene;
        }

        private void Update() // 슬라이드 기능 구현
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
        }

        private void OnEnable()
        {
            var star = GameObject.Find(SceneManager.GetActiveScene().name + "_" + ShowingQuestIndex);
            
            if (star != null) 
                star.GetComponent<BlinkStar>().OnClick();
        }

        private void OnLeftQuestBtnClick()
        {
            ShowingQuestIndex -= 1;

            Debug.Log(ShowingQuestIndex + ", Left");

            if (ShowingQuestIndex < _firstQuestsOf[1] &&
                SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Aries"))
            {
                AudioManager.GetInstance().ActSound();
                SceneManager.LoadScene("Aries");
                return;
            }

            var star = GameObject.Find(SceneManager.GetActiveScene().name + "_" + ShowingQuestIndex);
            if (star != null) star.GetComponent<BlinkStar>().ShowQuestInfo();
        }

        private void OnRightQuestBtnClick()
        {
            GameObject star;
            ShowingQuestIndex += 1;
            
            Debug.Log(ShowingQuestIndex + ", Right");
            
            if (ShowingQuestIndex > DataController.Instance.QuestProcess) //progress이 진행중 퀘스트
            {
                star = GameObject.Find(SceneManager.GetActiveScene().name + "_" + ShowingQuestIndex);
                
                if (star != null)
                    star.GetComponent<BlinkStar>().OnClick();
                
                ShowingQuestIndex = DataController.Instance.QuestProcess;
            }

            if (_firstQuestsOf[1] <= ShowingQuestIndex && ShowingQuestIndex < _firstQuestsOf[2])
                if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Taurus"))
                {
                    AudioManager.GetInstance().ActSound();
                    SceneManager.LoadScene("Taurus");
                    return;
                }

            star = GameObject.Find(SceneManager.GetActiveScene().name + "_" + ShowingQuestIndex);
            
            if (star != null)
                star.GetComponent<BlinkStar>().ShowQuestInfo();
        }
    }
}