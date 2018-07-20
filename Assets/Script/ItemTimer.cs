using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script
{
    public class ItemTimer : MonoBehaviour
    {
        public Button Btn;

        // 쿨타임 -> 타이머 쿨타임 업그레이드 추가 시 datacontroller에서 가져오는 걸로 수정 필요.
        // itemtimer 2,3도 마찬가지
        private const float Cooltime = 300.0f;

        public Image Img;
        public int Index;

        private DataController _leftTimer;

        private int _sec, _sec1, _sec10, _min;
        public Text TimeDisplayer; // 남은 시간 표시

        private void Awake()
        {
            _leftTimer = DataController.Instance;
        }

        private void Start()
        {
            if (Img == null)
                Img = gameObject.GetComponent<Image>();

            if (Btn == null)
                Btn = gameObject.GetComponent<Button>();
        }

        // 시간당 게이지 채우기, 남은 시간 표시
        private void Update()
        {
            if (_leftTimer[Index] > 0)
            {
                Btn.enabled = false;
                _sec = (int) _leftTimer[Index] % 60;
                _sec10 = _sec / 10;
                _sec1 = _sec % 10;
                _min = (int) _leftTimer[Index] / 60;
                TimeDisplayer.text = _min + ":" + _sec10 + _sec1;

                var ratio = 1.0f - _leftTimer[Index] / Cooltime;

                if (Img)
                    Img.fillAmount = ratio;
            }
            else
            {
                TimeDisplayer.text = "0:00";
                Img.fillAmount = 1.0f;

                _leftTimer[Index] = 0;

                if (Btn) Btn.enabled = true;
            }
        }

       
        public void ResetCooltime()
        {
            // 관찰자들에게 Click 이벤트 메세지 송출
            foreach (var target in _leftTimer.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));

            // 버튼 활성화 시
            if (!Btn) 
                return;
            
            // 세트 아이템 랜덤 생성
            var id = Random.Range(4001, 4059);

            while (id % 5 == 0) 
                id = Random.Range(4001, 4059);

            // 도감에 등록만 되면 됨
            _leftTimer.InsertNewItem(id); 
            AudioManager.Instance.ItemSound();
            PopUpWindow.Alert("[서적] " + DataDictionary.Instance.FindItemDic[id].Name + " 획득");

            _leftTimer[Index] = Cooltime;
            Btn.enabled = false;
        }
    }
}