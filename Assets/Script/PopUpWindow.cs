using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopUpWindow : MonoBehaviour
{

    private static bool isLocked;
    private static Queue alertQueue;

    public Text alertText;
    public Slider upgradeSlider;

    public float shakingTime;
    public float waitingTime;

    private static float ShakingTime;
    private static float WaitingTime;
    private static GameObject AlertPanel;
    private static Text AlertText;
    private static Slider UpgradeSlider;
    /*private static GameObject AlertPanelR;
    private static Text AlertTextR;*/
    // Use this for initialization
    void Awake()
    {
        Initialize();
        SceneManager.sceneLoaded +=(scene, mode)=>Initialize();
        alertQueue = new Queue();
        isLocked = false;
        ShakingTime = shakingTime > 0 ?shakingTime:0.5f;
        WaitingTime = waitingTime > 0 ? waitingTime : 0.3f;
    }

    private void Initialize()
    {
        GameObject gameObj=GameObject.Find("Alarm Window");
        if ( gameObj== null)
        {
            Debug.LogWarning("Alarm object is destroyed and not able to use alert.");
            return;
        }

        if (AlertPanel == null)
        {
            AlertPanel = gameObj;
        }
        if (alertText == null)
        {
            AlertText = AlertPanel.GetComponentInChildren<Text>();
        }
        else
        {
            AlertText = alertText;
        }
        /*if(AlertPanelR == null)
        {
            AlertPanelR = GameObject.Find("OnEventAlertR");
        }
        if (AlertTextR == null)
        {
            AlertTextR = AlertPanelR.GetComponentInChildren<Text>();
        }*/
        if (upgradeSlider == null)
        {
            UpgradeSlider = gameObj.GetComponentInChildren<Slider>();
            
        }
        else
        {
            UpgradeSlider = upgradeSlider;
        }
    }


    /// <summary>
    /// 화면에 PopUp알림을 띄웁니다.
    /// </summary>
    /// <param name="text">띄울 알림 문자열</param>
    /// <param name="obj">이 함수를 호출한 인스턴스 (this)</param>
    public static void Alert(string text, MonoBehaviour obj)
    {
        if (isLocked)
        {
            string temp=AlertText.text;
            AlertText.text = text;
            Debug.LogWarning("Queue is Locked.");
            obj.StartCoroutine(ChangeDialogueText(temp, 0.8f));
            return;
        }
        if (AlertPanel == null)
        {
            Debug.LogWarning("Alert Pannel is Null Object.");
            return;
        }
        alertQueue.Enqueue(text);

        if (alertQueue.Count <= 1)
        {
            GameObject alertPanel = AlertPanel;
            Text alertText = AlertText;
            Vector3 pos = alertPanel.transform.position;
            alertPanel.transform.position = new Vector3(540, pos.y, pos.z);
            obj.StartCoroutine(FadeOut(alertPanel.GetComponent<Image>(), alertText));
        }
    }

    /// <summary>
    /// 알림창을 숨깁니다. 큐 잠금을 풉니다.
    /// </summary>
    public static void HideDialogue()
    {
        if (AlertPanel == null)
        {
            Debug.LogWarning("Alert Pannel is Null Object.");
            return;
        }

        isLocked = false;

        AlertPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        AlertText.color = new Color(1, 1, 1, 0);
    }

    /// <summary>
    /// 알림창에 문자열을 띄웁니다. 충돌방지를 위해 사용중인 큐를 삭제하고 잠급니다.
    /// </summary>
    /// <param name="dialogue">알림창에 보여줄 문자열</param>
    public static void ShowDialogue(string dialogue)
    {
        if (AlertPanel == null)
        {
            Debug.LogWarning("Alert Pannel is Null Object.");
            return;
        }

        alertQueue.Clear();
        isLocked = true;

        AlertText.text = dialogue;
        AlertPanel.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        AlertText.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// 지연시간 안에 문자열만 바꿉니다.
    /// </summary>
    /// <param name="txt">새 문자열</param>
    /// <param name="latencySecond">지연시간</param>
    /// <returns></returns>
    public static IEnumerator ChangeDialogueText(string txt, float latencySecond)
    {

        yield return new WaitForSeconds(latencySecond);
       
        AlertText.text = txt;
    }
    /// <summary>
    /// value만큼의 비율을 갖도록 슬라이더를 띄웁니다.
    /// </summary>
    /// <param name="value">0~1사이 값</param>
    public static void SetSliderValue(float value)
    {
        if(UpgradeSlider != null)
        {
            if (!UpgradeSlider.gameObject.activeSelf) 
            {
                UpgradeSlider.gameObject.SetActive(true);
            }
           
            UpgradeSlider.value = value;
        }
        else
        {
            Debug.Log("Slider does not exist.");
        }
    }

    /// <summary>
    /// 슬라이더를 현재값에서 목표값으로 ShakingTime초의 요동치는 연출 이후 second동안 움직입니다.
    /// </summary>
    /// <param name="goalValue">목표값(0~1사이)</param>
    /// <param name="second">애니메이션 진행시간(초)</param>
    /// <param name="obj">이 함수를 호출한 인스턴스(this)</param>
    public static void AnimateSlider(float goalValue, float second, MonoBehaviour obj)
    {

        if (UpgradeSlider != null)
        {
            SetSliderValue(0.5f);
            obj.StartCoroutine(SliderAnimationCoroutine(goalValue, second));

        }
    }

    /// <summary>
    /// 슬라이더를 현재값에서 목표값으로 ShakingTime초의 요동치는 연출 이후 second동안 움직입니다. 작업이 끝나면 onComplete를 호출합니다.
    /// </summary>
    /// <param name="goalValue">목표값(0~1사이)</param>
    /// <param name="second">애니메이션 진행시간(초)</param>
    /// <param name="obj">이 함수를 호출한 인스턴스(this)</param>
    /// <param name="onComplete">호출할 함수 f(void)</param>
    public static void AnimateSlider(float goalValue, float second, MonoBehaviour obj, Action onComplete)
    {

        if (UpgradeSlider != null)
        {
            SetSliderValue(0.5f);
            obj.StartCoroutine(SliderAnimationCoroutine(goalValue, second, onComplete));            
        }
    }

    /// <summary>
    /// 슬라이더를 현재값에서 목표값으로 요동치지 않고 second동안 움직입니다. 작업이 끝나면 onComplete를 호출합니다.
    /// </summary>
    /// <param name="goalValue">목표값(0~1사이)</param>
    /// <param name="second">애니메이션 진행시간(초)</param>
    /// <param name="obj">이 함수를 호출한 인스턴스(this)</param>
    public static void AnimateSlider(float goalValue, float second, MonoBehaviour obj, bool normal)
    {
        if (UpgradeSlider != null)
        {
            if (!UpgradeSlider.gameObject.activeSelf)
            {
                UpgradeSlider.gameObject.SetActive(true);
            }
            obj.StartCoroutine(NormalSliderAnimationCoroutine(goalValue, second));
        }
    }

    /// <summary>
    /// 슬라이더를 현재값에서 목표값으로 요동치지 않고 없이 second동안 움직입니다. 작업이 끝나면 onComplete를 호출합니다.
    /// </summary>
    /// <param name="goalValue">목표값(0~1사이)</param>
    /// <param name="second">애니메이션 진행시간(초)</param>
    /// <param name="obj">이 함수를 호출한 인스턴스(this)</param>
    /// <param name="onComplete">호출할 함수 f(void)</param>
    public static void AnimateSlider(float goalValue,float second, MonoBehaviour obj, Action onComplete, bool normal)
    {
        if (UpgradeSlider != null)
        {
            if (!UpgradeSlider.gameObject.activeSelf)
            {
                UpgradeSlider.gameObject.SetActive(true);
            }
            obj.StartCoroutine(NormalSliderAnimationCoroutine(goalValue, second, onComplete));
        }
    }

    /// <summary>
    /// 슬라이더를 숨깁니다.
    /// </summary>
    public static void HideSlider()
    {
        if (UpgradeSlider != null)
        {
            UpgradeSlider.gameObject.SetActive(false);
        }
    }

    static IEnumerator FadeOut(Image img, Text txt)
    {
        while (alertQueue.Count > 0)
        {
            txt.text = (string)alertQueue.Peek();
            img.color = new Color(1, 1, 1, 1);
            txt.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(2.5f);
            for (float i = 0; i <= 1; i += 0.2f)
            {
                yield return new WaitForSeconds(0.1f);
                img.color = new Color(1, 1, 1, 1 - i);
                txt.color = new Color(1, 1, 1, 1 - i);
            }
            if (alertQueue.Count > 0)
            {
                alertQueue.Dequeue();
            }
        }

    }

    static IEnumerator SliderAnimationCoroutine(float goalValue, float second)
    {
        float amplitude = 0.25f;
        float diff = goalValue - amplitude;//목표값-현재값
        float deltaValue = diff / (10 * second);//1회 변화량
        yield return new WaitForSeconds(0.2f);

        for (float i = 0; i <= ShakingTime; i += 0.1f)
        {
            float rate = 2 * Mathf.PI * i / ShakingTime;
            UpgradeSlider.value = 0.5f + amplitude * (Mathf.Sin(rate));
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(WaitingTime);
        for (float i = 0; i <= second; i += 0.1f)
        {
            UpgradeSlider.value += deltaValue;
            yield return new WaitForSeconds(0.1f);
        }

    }

    static IEnumerator SliderAnimationCoroutine(float goalValue, float second, Action onComplete)
    {
        float amplitude = 0.25f;
        float diff = goalValue - amplitude;//목표값-현재값
        float deltaValue = diff / (10 * second);//1회 변화량
        yield return new WaitForSeconds(0.2f);

        for (float i = 0; i <= ShakingTime; i += 0.1f)
        {
            float rate = 2 * Mathf.PI * i / ShakingTime;
            UpgradeSlider.value = 0.5f+amplitude*(Mathf.Sin(rate));
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(WaitingTime);
        for (float i = 0; i <= second; i += 0.1f)
        {
            UpgradeSlider.value += deltaValue;
            yield return new WaitForSeconds(0.1f);
        }
        
        onComplete();
    }

    static IEnumerator NormalSliderAnimationCoroutine(float goalValue, float second)
    {
        float diff = goalValue - UpgradeSlider.value;//목표값-현재값
        float deltaValue = diff / (10 * second);//1회 변화량 
        yield return new WaitForSeconds(0.5f);
        for (float i = 0; i <= second; i += 0.1f)
        {
            UpgradeSlider.value += deltaValue;
            yield return new WaitForSeconds(0.1f);
        }

    }

    static IEnumerator NormalSliderAnimationCoroutine(float goalValue, float second, Action onComplete)
    {

        float diff = goalValue - UpgradeSlider.value;//목표값-현재값
        float deltaValue = diff / (10 * second);//1회 변화량
        yield return new WaitForSeconds(0.5f);
        for (float i = 0; i <= second; i += 0.1f)
        {
            UpgradeSlider.value += deltaValue;
            yield return new WaitForSeconds(0.1f);
        }

        onComplete();
    }

    private void OnEnable()//Awake 이후 재 검증
    {
        if (AlertPanel == null)
        {
            Initialize();
        }
    }

    private void OnDisable()
    {
        AlertPanel = null;
        AlertText = null;
    }


}
