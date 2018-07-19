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
                    
                    iTween.MoveTo(gameObject, iTween.Hash("x", transform.position.x - 1080.0f, 
                        "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
                    
                    MoveToLeft();
                }
                // 왼쪽에서 오른쪽 (->)
                else if (posXGap < 0 && NowSceneNum < DataController.MaxSceneNum - 1)
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
            // 관찰자들에게 Click 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this, 1));
            
            MoveToLeft();
        }

        public void OnClickRightBtn()
        {
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