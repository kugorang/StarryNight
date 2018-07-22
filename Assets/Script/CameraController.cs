using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Script
{
    public class CameraController : MonoBehaviour
    {
        private DataController _dataController;
        private int _minimumDiff;
        
        private float _startPosX;
        public static bool FocusOnItem { private get; set; }
        
        public static string NowScene
        {
            get { return SceneManager.GetActiveScene().name; }
        }
        
        public static int NowSceneNum
        {
            get
            {
                switch (NowScene)
                {
                    case "Main":
                        return PlayerPrefs.GetInt("MainSceneNum", 0);
                    case "QuestList":
                        return PlayerPrefs.GetInt("QuestListSceneNum", 0);
                    default:
                        return 0;
                }
            }
            private set 
            {
                switch (NowScene)
                {
                    case "Main":
                        PlayerPrefs.SetInt("MainSceneNum", value);
                        break;
                    case "QuestList":
                        PlayerPrefs.SetInt("QuestListSceneNum", value);
                        break;
                    default:
                        PlayerPrefs.SetInt("MainSceneNum", 0);
                        PlayerPrefs.SetInt("QuestListSceneNum", 0);
                        break;
                }
            }
        }


        
        /// <summary>
        /// 현재 씬의 화면 수를 반환. 1~12.
        /// </summary>
        public static int MaxSceneNum
        {
            get
            {
                switch (NowScene)
                {
                    case "Main":
                        return PlayerPrefs.GetInt("MainMaxNum", 2);
                    case "QuestList":
                        return PlayerPrefs.GetInt("QuestListMaxNum", 1);
                    default:
                        return 1;
                }
            }
        }

        /// <summary>
        /// Scene 이름을 받고 MaxSceneNum을 1 키웁니다.
        /// </summary>
        /// <param name="sceneName"></param>
        public static void AddScene(string sceneName)
        {
            int currentValue;
            switch (sceneName)
                {
                    case "Main":
                        currentValue = PlayerPrefs.GetInt("MainMaxNum", 2);
                        PlayerPrefs.SetInt("MainMaxNum", currentValue+1);
                        break;
                    case "QuestList":
                        currentValue = PlayerPrefs.GetInt("QuestListMaxNum", 1);
                    if (currentValue >= 12) return;//별자리는 총 12개이므로, MaxSceneNum은 12까지.                       
                        PlayerPrefs.SetInt("QuestListMaxNum", currentValue+1);
                        break;
                    default:
                        PlayerPrefs.SetInt("MainMaxNum", 1);
                        PlayerPrefs.SetInt("QuestListMaxNum", 1);
                        break;
                }
        }

        private void Awake()
        {
            FocusOnItem = false;
            _minimumDiff = Screen.width / 8;
            _dataController = DataController.Instance;
            
            transform.position = new Vector3(transform.position.x + NowSceneNum * 1080.0f, transform.position.y, transform.position.z);
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !FocusOnItem)
            {
                _startPosX = Input.mousePosition.x;
            }
            else if (Input.GetMouseButtonUp(0) && !FocusOnItem)
            {
                var posXGap = Input.mousePosition.x - _startPosX;

                if (!(Math.Abs(posXGap) > _minimumDiff))
                    return;

                // 오른쪽에서 왼쪽 (<-)
                if (posXGap > 0 && NowSceneNum > 0)
                {
                    // 관찰자들에게 Slide 이벤트 메세지 송출
                    foreach (var target in _dataController.Observers) 
                        ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnSlide(true, 1));
                    
                    MoveToLeft();
                }
                // 왼쪽에서 오른쪽 (->)
                else if (posXGap < 0 && NowSceneNum < MaxSceneNum - 1)
                {
                    // 관찰자들에게 Slide 이벤트 메세지 송출
                    foreach (var target in _dataController.Observers) 
                        ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnSlide(false, 1));

                    MoveToRight();
                }
            }
        }

        public void OnClickLeftBtn()
        {
            if (NowSceneNum <= 0) return;
            // 관찰자들에게 Click 이벤트 메세지 송출
            foreach (var target in _dataController.Observers)
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this, 1));

            MoveToLeft();
        }

        public void OnClickRightBtn()
        {
            if (NowSceneNum >= MaxSceneNum - 1) return;
            // 관찰자들에게 Click 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this, 0));
            
            MoveToRight();
        }

        private void MoveToLeft()
        {
            iTween.MoveTo(gameObject, iTween.Hash("x", transform.position.x - 1080.0f, 
                "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
            
            --NowSceneNum;
        }

        private void MoveToRight()
        {
            iTween.MoveTo(gameObject, iTween.Hash("x", transform.position.x + 1080.0f, 
                "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
            
            ++NowSceneNum;
        }
    }
}