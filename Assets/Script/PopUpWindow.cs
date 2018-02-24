using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopUpWindow : MonoBehaviour {

    private static bool isLocked;
    private static Queue alertQueue;

    public Text alertText;
    public Slider upgradeSlider;

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
    }

    private void Initialize()
    {
        if (AlertPanel == null)
        {
            AlertPanel = gameObject;
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
            UpgradeSlider = gameObject.GetComponentInChildren<Slider>();
            
        }
        else
        {
            UpgradeSlider = upgradeSlider;
        }
    }


    /// <summary>
    /// 왼쪽 화면에 PopUp알림을 띄웁니다.
    /// </summary>
    /// <param name="text">띄울 알림 문자열</param>
    /// <param name="obj">이 함수를 호출한 인스턴스 (this)</param>
    public static void Alert(string text, MonoBehaviour obj)
    {
        Alert(text, obj, false);
    }

    /// <summary>
    /// 선택한 화면에 PopUp알림을 띄웁니다.
    /// </summary>
    /// <param name="text">띄울 알림 문자열</param>
    /// <param name="obj">이 함수를 호출한 인스턴스 (this)</param>
    /// <param name="isRight">오른쪽->true</param>
    public static void Alert(string text, MonoBehaviour obj, bool isRight)
    {
        if (isLocked)
        {
            Debug.Log("Queue is Locked.");
            return;
        }
        if (AlertPanel == null)
        {
            Debug.Log("Alert Pannel is Null Object.");
            return;
        }
        alertQueue.Enqueue(text);
        Debug.Log(alertQueue.Count + " count of queue!! " + alertQueue.Peek());
        if (alertQueue.Count <= 1)
        {
            GameObject alertPanel = AlertPanel;
            Text alertText = AlertText;
            Vector3 pos = alertPanel.transform.position;
            if (isRight && pos.x < 0)
            {
                /*   alertPanel = AlertPanelR;
                   alertText = AlertTextR;*/

                alertPanel.transform.position = new Vector3(540, pos.y, pos.z);
            }
            else if (!isRight && pos.x > 0)
            {
                alertPanel.transform.position = new Vector3(-540, pos.y, pos.z);
            }
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
            Debug.Log("Alert Pannel is Null Object.");
            return;
        }

        alertQueue.Clear();
        isLocked = true;

        AlertText.text = dialogue;
        AlertPanel.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        AlertText.color = new Color(1, 1, 1, 1);
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
    }

    /// <summary>
    /// 슬라이더를 현재값에서 목표값으로 일정시간동안 움직입니다.
    /// </summary>
    /// <param name="goalValue">목표값(0~1사이)</param>
    /// <param name="second">애니메이션 진행시간(초)</param>
    /// <param name="obj">이 함수를 호출한 인스턴스(this)</param>
    public static void AnimateSlider(float goalValue, float second, MonoBehaviour obj)
    {

        if (UpgradeSlider != null)
        {
            if (!UpgradeSlider.gameObject.activeSelf)
            {
                UpgradeSlider.gameObject.SetActive(true);
            }
            obj.StartCoroutine(SliderAnimationCoroutine(goalValue, second));

        }
    }

    /// <summary>
    /// 슬라이더를 현재값에서 목표값으로 일정시간동안 움직입니다. 작업이 끝나면 onComplete를 호출합니다.
    /// </summary>
    /// <param name="goalValue">목표값(0~1사이)</param>
    /// <param name="second">애니메이션 진행시간(초)</param>
    /// <param name="obj">이 함수를 호출한 인스턴스(this)</param>
    /// <param name="onComplete">호출할 함수 f(void)</param>
    public static void AnimateSlider(float goalValue, float second, MonoBehaviour obj, Action onComplete)
    {

        if (UpgradeSlider != null)
        {
            if (!UpgradeSlider.gameObject.activeSelf)
            {
                UpgradeSlider.gameObject.SetActive(true);
            }
            obj.StartCoroutine(SliderAnimationCoroutine(goalValue, second, onComplete));            
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
            Debug.Log(alertQueue.Count + " count of queue " + alertQueue.Peek());
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
            alertQueue.Dequeue();
        }

    }

    static IEnumerator SliderAnimationCoroutine(float goalValue, float second)
    {
        float diff = goalValue-UpgradeSlider.value;//목표값-현재값
        float deltaValue = diff/(10*second);//1회 변화량 
        yield return new WaitForSeconds(0.5f);
        for (float i = 0; i <= second; i += 0.1f)
        {
            UpgradeSlider.value += deltaValue;
            yield return new WaitForSeconds(0.1f);         
        }
        
    }

    static IEnumerator SliderAnimationCoroutine(float goalValue, float second, Action onComplete)
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

    private void OnDestroy()
    {
        AlertPanel = null;
        AlertText = null;
    }


}
