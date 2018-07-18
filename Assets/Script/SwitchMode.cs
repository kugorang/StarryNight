using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script
{
    public class SwitchMode : MonoBehaviour
    {
        private DataController _dataController;
        public Sprite Star, Tree;
        public Button Button;

        private static SwitchMode _instance;

        public static SwitchMode Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                
                _instance = FindObjectOfType<SwitchMode>();

                if (_instance != null) 
                    return _instance;
                
                var container = new GameObject("SwitchMode");

                _instance = container.AddComponent<SwitchMode>();

                return _instance;
            }
        }

        /// <summary>
        ///     현재 스위치 상태 가져오기
        ///     true이면 별, false이면 다른 재료
        /// </summary>
        public bool State
        {
            get { return Convert.ToBoolean(_dataController.SwitchButtonMode); }
            private set
            {
                var nowState = Convert.ToBoolean(value);
                
                gameObject.GetComponent<Image>().sprite = nowState ? Tree : Star;
                _dataController.SwitchButtonMode = nowState ? 1 : 0;
            }
        }

        private void Start()
        {
            if (Button == null) 
                Button = gameObject.GetComponent<Button>();

            _dataController = DataController.Instance;
            gameObject.GetComponent<Image>().sprite = State ? Tree : Star;
        }

        // 버튼 스위치
        public void CheckButton()
        {
            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));

            // true가 sun, false가 moon
            State = !State;
        }
    }
}