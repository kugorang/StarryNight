using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script
{
    public class ItemTimer : MonoBehaviour, IResetables
    {
        public Button Btn;
        public Image Img;
        public int Index;
        public Text TimeDisplayer; // 남은 시간 표시

        private DataController _dataController;

        // 쿨타임 -> 타이머 쿨타임 업그레이드 추가 시 datacontroller에서 가져오는 걸로 수정 필요.
        // itemtimer 2,3도 마찬가지
        private const float Cooltime = 300.0f;

        private float _reducedCooltime;
        // 아이템 타이머 시간
        private float LeftTimer
        {
            get { return PlayerPrefs.GetFloat("LeftTimer" + (Index + 1), Cooltime); }//LeftTimer1, LeftTimer2, LeftTimer3이므로 index+1
            set
            {
                PlayerPrefs.SetFloat("LeftTimer" + (Index + 1), value);
            }
        }

        // 플레이 종료 시간 가져오기
        private static DateTime LastPlayDate
        {
            get
            {
                return !PlayerPrefs.HasKey("Time") ? DateTime.Now : DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("Time")));
            }
        }

        // 종료 후 지난 시간 계산
        private static int TimeAfterLastPlay
        {
            get
            {
                var currentTime = DateTime.Now;
                var lastTime = LastPlayDate;

                var subTime = (int)currentTime.Subtract(lastTime).TotalSeconds;
                return subTime;
            }
        }

        private void Start()
        {
            _dataController=DataController.Instance;
            if (!_dataController.ResetList.Contains(gameObject))
                _dataController.ResetList.Add(gameObject);
            _reducedCooltime = Cooltime - _dataController.CoolTimeReduction(Index)*_dataController.TwiceAll;
            LeftTimer = (LeftTimer>0)?_reducedCooltime-TimeAfterLastPlay:0;
            if (Img == null)
                Img = gameObject.GetComponent<Image>();

            if (Btn == null)
                Btn = gameObject.GetComponent<Button>();
        }


        // 시간당 게이지 채우기, 남은 시간 표시
        private void Update()
        {
           
            LeftTimer -= Time.deltaTime;
            
            if (LeftTimer > 0)
            {
                Btn.enabled = false;
                var sec = (int) LeftTimer % 60;
                var sec10 = sec / 10;
                var sec1 = sec % 10;
                var min = (int) LeftTimer / 60;
                TimeDisplayer.text = min + ":" + sec10 + sec1;

                var ratio = 1.0f - LeftTimer/ Cooltime;

                if (Img)
                    Img.fillAmount = ratio;
            }
            else
            {
                TimeDisplayer.text = "0:00";
                Img.fillAmount = 1.0f;

                LeftTimer = 0;

                if (Btn) Btn.enabled = true;
            }
        }

       
        public void ResetCooltime()//TODO:더좋은 아이템 나올 확률 또는 아이템 2개 나올확률 추가.
        {
            // 관찰자들에게 Click 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));

            // 버튼 활성화 시
            if (!Btn) 
                return;
            
            // 세트 아이템 랜덤 생성
            var id = Random.Range(4001, 4059);

            while (id % 5 == 0) 
                id = Random.Range(4001, 4059);

            // 도감에 등록만 되면 됨
            _dataController.InsertNewItem(id); 
            AudioManager.Instance.ItemSound();
            PopUpWindow.Alert("[서적] " + DataDictionary.Instance.FindItemDic[id].Name + " 획득");

            LeftTimer = _reducedCooltime;
            Btn.enabled = false;
        }

        public void OnReset()
        { // 리셋 즉시 시험할 수 있게
            LeftTimer = 0f;
        }

        public void OnDisable()
        {
            _dataController.ResetList.Remove(gameObject);
        }

    }
}