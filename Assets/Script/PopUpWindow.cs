using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script
{
    public class PopUpWindow : MonoBehaviour
    {
        private static bool _isLocked;
        private static float _shakingTime, _waitingTime;
        private static Text _alertText;
        private static Queue _alertQueue;
        private static Slider _upgradeSlider;
        private static GameObject _alertPanel;

        public Text AlertText;
        public float WaitingTime, ShakingTime;
        public Slider UpgradeSlider;

        /*private static GameObject AlertPanelR;
    private static Text AlertTextR;*/
        
        // Use this for initialization
        private void Awake()
        {
            Initialize();
            SceneManager.sceneLoaded += (scene, mode) => Initialize();
            _alertQueue = new Queue();
            _isLocked = false;
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
        }

        /// <summary>
        ///     화면에 PopUp알림을 띄웁니다.
        /// </summary>
        /// <param name="text">띄울 알림 문자열</param>
        /// <param name="obj">이 함수를 호출한 인스턴스 (this)</param>
        public static void Alert(string text, MonoBehaviour obj)
        {
            if (_isLocked)
            {
                var temp = _alertText.text;
                _alertText.text = text;
                
                Debug.LogWarning("Queue is Locked.");
                
                obj.StartCoroutine(ChangeDialogueText(temp, 0.8f));
                
                return;
            }

            if (_alertPanel == null)
            {
                Debug.LogWarning("Alert Pannel is Null Object.");
                return;
            }

            _alertQueue.Enqueue(text);

            if (_alertQueue.Count > 1) 
                return;
            
            var alertPanel = _alertPanel;
            var alertText = _alertText;
            var pos = alertPanel.transform.position;
            
            alertPanel.transform.position = new Vector3(540, pos.y, pos.z);
            obj.StartCoroutine(FadeOut(alertPanel.GetComponent<Image>(), alertText));
        }

        /// <summary>
        ///     알림창을 숨깁니다. 큐 잠금을 풉니다.
        /// </summary>
        public static void HideDialogue()
        {
            if (_alertPanel == null)
            {
                Debug.LogWarning("Alert Pannel is Null Object.");
                return;
            }

            _isLocked = false;

            _alertPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            _alertText.color = new Color(1, 1, 1, 0);
        }

        /// <summary>
        ///     알림창에 문자열을 띄웁니다. 충돌방지를 위해 사용중인 큐를 삭제하고 잠급니다.
        /// </summary>
        /// <param name="dialogue">알림창에 보여줄 문자열</param>
        public static void ShowDialogue(string dialogue)
        {
            if (_alertPanel == null)
            {
                Debug.LogWarning("Alert Pannel is Null Object.");
                return;
            }

            _alertQueue.Clear();
            _isLocked = true;

            _alertText.text = dialogue;
            _alertPanel.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            _alertText.color = new Color(1, 1, 1, 1);
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
        /// <param name="obj">이 함수를 호출한 인스턴스(this)</param>
        public static void AnimateSlider(float goalValue, float second, MonoBehaviour obj)
        {
            if (_upgradeSlider == null) 
                return;
            
            SetSliderValue(0.5f);
            obj.StartCoroutine(SliderAnimationCoroutine(goalValue, second));
        }

        /// <summary>
        ///     슬라이더를 현재값에서 목표값으로 ShakingTime초의 요동치는 연출 이후 second동안 움직입니다. 작업이 끝나면 onComplete를 호출합니다.
        /// </summary>
        /// <param name="goalValue">목표값(0~1사이)</param>
        /// <param name="second">애니메이션 진행시간(초)</param>
        /// <param name="obj">이 함수를 호출한 인스턴스(this)</param>
        /// <param name="onComplete">호출할 함수 f(void)</param>
        public static void AnimateSlider(float goalValue, float second, MonoBehaviour obj, Action onComplete)
        {
            if (_upgradeSlider == null) 
                return;
            
            SetSliderValue(0.5f);
            obj.StartCoroutine(SliderAnimationCoroutine(goalValue, second, onComplete));
        }

        /// <summary>
        ///     슬라이더를 현재값에서 목표값으로 요동치지 않고 second동안 움직입니다. 작업이 끝나면 onComplete를 호출합니다.
        /// </summary>
        /// <param name="goalValue">목표값(0~1사이)</param>
        /// <param name="second">애니메이션 진행시간(초)</param>
        /// <param name="obj">이 함수를 호출한 인스턴스(this)</param>
        /// <param name="normal"></param>
        public static void AnimateSlider(float goalValue, float second, MonoBehaviour obj, bool normal)
        {
            if (_upgradeSlider == null) 
                return;
            
            if (!_upgradeSlider.gameObject.activeSelf) 
                _upgradeSlider.gameObject.SetActive(true);
            
            obj.StartCoroutine(NormalSliderAnimationCoroutine(goalValue, second));
        }

        /// <summary>
        ///     슬라이더를 현재값에서 목표값으로 요동치지 않고 없이 second동안 움직입니다.
        ///     작업이 끝나면 onComplete를 호출합니다.
        /// </summary>
        /// <param name="goalValue">목표값(0~1사이)</param>
        /// <param name="second">애니메이션 진행시간(초)</param>
        /// <param name="obj">이 함수를 호출한 인스턴스(this)</param>
        /// <param name="onComplete">호출할 함수 f(void)</param>
        /// <param name="normal"></param>
        public static void AnimateSlider(float goalValue, float second, MonoBehaviour obj, Action onComplete, bool normal)
        {
            if (_upgradeSlider == null) 
                return;
            
            if (!_upgradeSlider.gameObject.activeSelf) 
                _upgradeSlider.gameObject.SetActive(true);
            
            obj.StartCoroutine(NormalSliderAnimationCoroutine(goalValue, second, onComplete));
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

                if (_alertQueue.Count > 0) 
                    _alertQueue.Dequeue();
            }
        }

        private static IEnumerator SliderAnimationCoroutine(float goalValue, float second)
        {
            const float amplitude = 0.25f;
            var diff = goalValue - amplitude; // 목표값 - 현재값
            var deltaValue = diff / (10 * second); // 1회 변화량
            
            yield return new WaitForSeconds(0.2f);

            for (float i = 0; i <= _shakingTime; i += 0.1f)
            {
                var rate = 2 * Mathf.PI * i / _shakingTime;
                _upgradeSlider.value = 0.5f + amplitude * Mathf.Sin(rate);
                
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(_waitingTime);
            
            for (float i = 0; i <= second; i += 0.1f)
            {
                _upgradeSlider.value += deltaValue;
                
                yield return new WaitForSeconds(0.1f);
            }
        }

        private static IEnumerator SliderAnimationCoroutine(float goalValue, float second, Action onComplete)
        {
            const float amplitude = 0.25f;
            var diff = goalValue - amplitude;              // 목표값 - 현재값
            var deltaValue = diff / (10 * second);         // 1회 변화량
            
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

            onComplete();
        }

        private static IEnumerator NormalSliderAnimationCoroutine(float goalValue, float second)
        {
            var diff = goalValue - _upgradeSlider.value;   // 목표값 - 현재값
            var deltaValue = diff / (10 * second);         // 1회 변화량
            
            yield return new WaitForSeconds(0.5f);
            
            for (float i = 0; i <= second; i += 0.1f)
            {
                _upgradeSlider.value += deltaValue;
                
                yield return new WaitForSeconds(0.1f);
            }
        }

        private static IEnumerator NormalSliderAnimationCoroutine(float goalValue, float second, Action onComplete)
        {
            var diff = goalValue - _upgradeSlider.value; // 목표값 - 현재값
            var deltaValue = diff / (10 * second);       // 1회 변화량
            
            yield return new WaitForSeconds(0.5f);
            
            for (float i = 0; i <= second; i += 0.1f)
            {
                _upgradeSlider.value += deltaValue;
                yield return new WaitForSeconds(0.1f);
            }

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