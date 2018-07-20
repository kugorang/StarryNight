using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script
{
    public class PopUpWindow : MonoBehaviour
    {
        private static float _shakingTime, _waitingTime;
        private static string _formerText="";
        private static Text _alertText;
        private static Queue _alertQueue;
        private static Slider _upgradeSlider;
        private static GameObject _alertPanel;
        private static Image _blockTouchImage;//Raycast Target을 켜서 중요한 알람 도중 다른 조작을 막기위한 패널(의 이미지 컴포넌트.)
        private static PopUpWindow _this;

        public Text AlertText;
        public float WaitingTime, ShakingTime;
        public Slider UpgradeSlider;

        public static bool IsAlerting
        {
            get { return _alertQueue.Count > 0; }
        }


        
        // Use this for initialization
        private void Awake()
        {
            Initialize();
            _alertQueue = new Queue();
            _shakingTime = ShakingTime > 0 ? ShakingTime : 0.5f;
            _waitingTime = WaitingTime > 0 ? WaitingTime : 0.3f;
        }

        private void Initialize()
        {
           var gameObj = GameObject.Find("Alarm Window");
            
            if (gameObj == null)
            {
                Debug.LogWarning("Alarm object is destroyed and not able to use alert.");
                return;
            }

            if (_alertPanel == null) 
                _alertPanel = gameObj;

            _alertText = AlertText == null ? _alertPanel.GetComponentInChildren<Text>() : AlertText;
            _upgradeSlider = UpgradeSlider == null ? gameObj.GetComponentInChildren<Slider>() : UpgradeSlider;
            _this = gameObj.GetComponent<PopUpWindow>();
            _blockTouchImage = gameObj.transform.parent.parent.gameObject.GetComponent<Image>();//UI&DialogPanel(임시)은 AlarmWindow의 할아버지
        }

        /// <summary>
        ///     화면에 PopUp알림을 띄웁니다.
        /// </summary>
        /// <param name="text">띄울 알림 문자열</param>
        public static void Alert(string text)//TODO: 동일한 요청 여러 번 올시 Fade하지 않거나 무시하는 기능 구현
        {
          

            if (_alertPanel == null)
            {
                Debug.LogWarning("Alert Pannel is Null Object.");
                return;
            }

            if (_formerText != text)//HACK
            {
                _alertQueue.Enqueue(text);
            }

            _formerText = text;
            if (_alertQueue.Count > 1)
            {
                return;
            }
            
            _this.StartCoroutine(FadeOut(_alertPanel.GetComponent<Image>(), _alertText));
        }

       

        /// <summary>
        ///     지연시간 안에 문자열만 바꿉니다.
        /// </summary>
        /// <param name="txt">새 문자열</param>
        /// <param name="latencySecond">지연시간</param>
        /// <returns></returns>
        private static IEnumerator ChangeDialogueText(string txt, float latencySecond)
        {
            yield return new WaitForSeconds(latencySecond);

            _alertText.text = txt;
        }

        /// <summary>
        ///     value 만큼의 비율을 갖도록 슬라이더를 띄웁니다.
        /// </summary>
        /// <param name="value">0 ~ 1 사이 값</param>
        public static void SetSliderValue(float value)
        {
            if (_upgradeSlider != null)
            {
                if (!_upgradeSlider.gameObject.activeSelf) 
                    _upgradeSlider.gameObject.SetActive(true);

                _upgradeSlider.value = value;
            }
            else
            {
                Debug.Log("Slider does not exist.");
            }
        }

        /// <summary>
        ///     슬라이더를 현재값에서 목표값으로 ShakingTime 초의 요동치는 연출 이후 second 동안 움직입니다.
        /// </summary>
        /// <param name="goalValue">목표값(0~1사이)</param>
        /// <param name="second">애니메이션 진행시간(초)</param>
        public static void AnimateSlider(float goalValue, float second)
        {
            if (_upgradeSlider == null) 
                return;
            
            SetSliderValue(0.5f);
            _this.StartCoroutine(SliderAnimationCoroutine(goalValue, second, () => { }));
        }

        /// <summary>
        ///     슬라이더를 현재값에서 목표값으로 ShakingTime초의 요동치는 연출 이후 second동안 움직입니다. 작업이 끝나면 onComplete를 호출합니다.
        /// </summary>
        /// <param name="goalValue">목표값(0~1사이)</param>
        /// <param name="second">애니메이션 진행시간(초)</param>
        /// <param name="onComplete">호출할 함수 f(void)</param>
        public static void AnimateSlider(float goalValue, float second, Action onComplete)
        {
            if (_upgradeSlider == null) 
                return;
            
            SetSliderValue(0.5f);
            _this.StartCoroutine(SliderAnimationCoroutine(goalValue, second, onComplete));
        }

        /// <summary>
        ///     슬라이더를 현재값에서 목표값으로 요동치지 않고 second동안 움직입니다. 작업이 끝나면 onComplete를 호출합니다.
        /// </summary>
        /// <param name="goalValue">목표값(0~1사이)</param>
        /// <param name="second">애니메이션 진행시간(초)</param>
        /// <param name="normal"></param>
        public static void AnimateSlider(float goalValue, float second, bool normal)
        {
            if (_upgradeSlider == null) 
                return;
            
            if (!_upgradeSlider.gameObject.activeSelf) 
                _upgradeSlider.gameObject.SetActive(true);
            
            _this.StartCoroutine(NormalSliderAnimationCoroutine(goalValue, second, () => { }));
        }

        /// <summary>
        ///     슬라이더를 현재값에서 목표값으로 요동치지 않고 없이 second동안 움직입니다.
        ///     작업이 끝나면 onComplete를 호출합니다.
        /// </summary>
        /// <param name="goalValue">목표값(0~1사이)</param>
        /// <param name="second">애니메이션 진행시간(초)</param>
        /// <param name="onComplete">호출할 함수 f(void)</param>
        /// <param name="normal"></param>
        public static void AnimateSlider(float goalValue, float second, Action onComplete, bool normal)
        {
            if (_upgradeSlider == null) 
                return;
            
            if (!_upgradeSlider.gameObject.activeSelf) 
                _upgradeSlider.gameObject.SetActive(true);
            
            _this.StartCoroutine(NormalSliderAnimationCoroutine(goalValue, second, onComplete));
        }

        /// <summary>
        ///     슬라이더를 숨깁니다.
        /// </summary>
        public static void HideSlider()
        {
            if (_upgradeSlider != null)
                _upgradeSlider.gameObject.SetActive(false);
        }

        private static IEnumerator FadeOut(Graphic img, Text txt)
        {
            while (_alertQueue.Count > 0)
            {
                txt.text = (string) _alertQueue.Peek();
                img.color = new Color(1, 1, 1, 1);
                txt.color = new Color(1, 1, 1, 1);
                
                yield return new WaitForSeconds(2.5f);
                
                for (float i = 0; i <= 1; i += 0.2f)
                {
                    yield return new WaitForSeconds(0.1f);
                    img.color = new Color(1, 1, 1, 1 - i);
                    txt.color = new Color(1, 1, 1, 1 - i);
                }
                if (_alertQueue.Count > 0)//동기화 오류로 추정되는 문제로 인해 검증 구문이 필요함. InvalidOperationException: Operation is not valid due to the current state of the object
                { 
                 _alertQueue.Dequeue();//작업이 끝난 후에 Dequeue하지 않으면, Fade 중에 _alertQueue.Count=0이 되어 Alert에 오류가 발생
                }
            }
        }


        private static IEnumerator SliderAnimationCoroutine(float goalValue, float second, Action onComplete)
        {
            const float amplitude = 0.25f;
            var diff = goalValue - amplitude;              // 목표값 - 현재값
            var deltaValue = diff / (10 * second);         // 1회 변화량

            _blockTouchImage.raycastTarget = true;
            yield return new WaitForSeconds(0.2f);

            for (float i = 0; i <= _shakingTime; i += 0.03f)
            {
                var rate = 2 * Mathf.PI * i / _shakingTime;
                _upgradeSlider.value = 0.5f + amplitude * Mathf.Sin(rate);
                
                yield return new WaitForSeconds(0.03f);
            }

            yield return new WaitForSeconds(_waitingTime);
            
            for (float i = 0; i <= second; i += 0.03f)
            {
                _upgradeSlider.value += deltaValue;
                
                yield return new WaitForSeconds(0.03f);
            }
            _blockTouchImage.raycastTarget = false;
            HideSlider();
            onComplete();
        }

        private static IEnumerator NormalSliderAnimationCoroutine(float goalValue, float second, Action onComplete)
        {
            var diff = goalValue - _upgradeSlider.value; // 목표값 - 현재값
            var deltaValue = diff / (10 * second);       // 1회 변화량
            _blockTouchImage.raycastTarget = true;
            yield return new WaitForSeconds(0.5f);
            
            for (float i = 0; i <= second; i += 0.1f)
            {
                _upgradeSlider.value += deltaValue;
                yield return new WaitForSeconds(0.1f);
            }
            _blockTouchImage.raycastTarget = false;
            HideSlider();
            onComplete();
        }

        private void OnEnable()                         // Awake 이후 재 검증
        {
            if (_alertPanel == null) 
                Initialize();
        }

        private void OnDisable()
        {
            _alertPanel = null;
            _alertText = null;
        }
    }
}