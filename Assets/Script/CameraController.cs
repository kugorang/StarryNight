using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Assets.Script
{
    public class CameraController : MonoBehaviour
    {
        private DataController _dataController;
        private Scene _nowScene;
        private int _nowSceneNum, _minimumDiff;
        private float _startPosX;
        
        public GameObject MainCamera;
        public int MaxSceneNum;

        // 210 이상, 시작 위치가 지구본과 겹치지 않게.
        [Tooltip("지구본과 겹치지 않게 적절한 X를 주세요.")] 
        public float MinimumX;

        public static bool FocusOnItem { private get; set; }

        private int LastScene 
        {
            get
            {
                return PlayerPrefs.GetInt(_nowScene.name == "Main" ? "MainLastScene" : "QuestLastScene", 0);
            }
            set
            {
                PlayerPrefs.SetInt(_nowScene.name == "Main" ? "MainLastScene" : "QuestLastScene", value);
            }
        }

        private void Awake()
        {
            FocusOnItem = false;
            _minimumDiff = Screen.width / 8;
            _dataController = DataController.Instance;
            _nowScene = SceneManager.GetActiveScene();
            _nowSceneNum = LastScene;
            
            transform.position = new Vector3(transform.position.x + LastScene * 1080.0f, transform.position.y, transform.position.z);
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
                if (posXGap > 0 && _nowSceneNum > 0)
                {
                    // 관찰자들에게 Slide 이벤트 메세지 송출
                    foreach (var target in _dataController.Observers) 
                        ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnSlide(true, 1));
                    
                    iTween.MoveTo(gameObject, iTween.Hash("x", transform.position.x - 1080.0f, 
                        "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
                    LastScene = --_nowSceneNum;
                }
                // 왼쪽에서 오른쪽 (->)
                else if (posXGap < 0 && _startPosX > MinimumX && _nowSceneNum < MaxSceneNum - 1)
                {
                    // 관찰자들에게 Slide 이벤트 메세지 송출
                    foreach (var target in _dataController.Observers) 
                        ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnSlide(false, 1));

                    iTween.MoveTo(gameObject, iTween.Hash("x", transform.position.x + 1080.0f, 
                        "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
                    LastScene += ++_nowSceneNum;
                }
            }
        }

        public void OnClickLeftBtn()
        {
            // 관찰자들에게 Click 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this, 1));
            
            iTween.MoveTo(MainCamera, iTween.Hash("x", transform.position.x - 1080.0f, 
                "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
            LastScene = --_nowSceneNum;
        }

        public void OnClickRightBtn()
        {
            // 관찰자들에게 Click 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this, 0));
            
            iTween.MoveTo(MainCamera, iTween.Hash("x", transform.position.x + 1080.0f, 
                "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
            LastScene = ++_nowSceneNum;
        }

        public void OnApplicationQuit()
        {
            // 앱 종료시 현재 표시하고 있는 화면인 상태로 종료
            LastScene = _nowSceneNum; 
        }
    }
}