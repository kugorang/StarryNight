using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script
{
    public class SwitchSunMoon : MonoBehaviour
    {
        private static SwitchSunMoon _instance;
        private DataController _dataController;
        public Sprite Moon, Sun;
        private bool _state;

        public Button SunMoonbtn;

        public static SwitchSunMoon Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                
                _instance = FindObjectOfType<SwitchSunMoon>();

                if (_instance != null) 
                    return _instance;
                
                var container = new GameObject("SwitchSunMoon");

                _instance = container.AddComponent<SwitchSunMoon>();

                return _instance;
            }
        }

        /// <summary>
        ///     현재 스위치 상태 가져오기
        ///     true이면 별, false이면 다른 재료
        /// </summary>
        public bool State
        {
            get { return _state; }

            private set
            {
                gameObject.GetComponent<Image>().sprite = value ? Sun : Moon;
                _state = value;
            }
        }

        private void Start()
        {
            if (SunMoonbtn == null) 
                SunMoonbtn = gameObject.GetComponent<Button>();

            _dataController = DataController.Instance;
        }

        // 버튼 스위치
        public void CheckButton()
        {
            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));

            // true가 sun, false가 moon
            State = !_state; 
        }
    }
}