using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PopUpAlert : MonoBehaviour {

    private static bool isFading;
    private static Queue alertQueue;

    [SerializeField]
    private static GameObject AlertPanel;
    private static Text AlertText;
    private static GameObject AlertPanelR;
    private static Text AlertTextR;
    // Use this for initialization
    void Start () {
        if (AlertPanel == null)
        {
            AlertPanel = gameObject;
        }
        if (AlertText == null)
        {
            AlertText = AlertPanel.GetComponentInChildren<Text>();
        }
        if(AlertPanelR == null)
        {
            AlertPanelR = GameObject.Find("OnEventAlertR");
        }
        if (AlertTextR == null)
        {
            AlertTextR = AlertPanelR.GetComponentInChildren<Text>();
        }
        alertQueue = new Queue();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 왼쪽 화면에 PopUp알림을 띄웁니다.
    /// </summary>
    /// <param name="text">띄울 알림 문자열</param>
    /// <param name="obj">이 함수를 호출한 인스턴스 (this)</param>
    public static void Alert(string text,MonoBehaviour obj)
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
        alertQueue.Enqueue(text);
        Debug.Log(alertQueue.Count + " count of queue!! "+alertQueue.Peek());
        if (alertQueue.Count <= 1)
        {
            GameObject alertPanel = AlertPanel;
            Text alertText = AlertText;
            if (isRight)
            {
                alertPanel = AlertPanelR;
                alertText = AlertTextR;
            }
            obj.StartCoroutine(FadeOut(alertPanel.GetComponent<Image>(), alertText));
        }
    }

    static IEnumerator FadeOut(Image img, Text txt)
    {
        while (alertQueue.Count > 0)
        {
            Debug.Log(alertQueue.Count + " count of queue " + alertQueue.Peek());
            txt.text = (string)alertQueue.Peek();
            img.color = new Color(1, 1, 1, 0.5f);
            txt.color = new Color(0, 0, 0, 1);
            yield return new WaitForSeconds(0.5f);
            for (float i = 0; i <= 1; i += 0.2f)
            {
                yield return new WaitForSeconds(0.1f);
                img.color = new Color(1, 1, 1, 0.5f - i / 2);
                txt.color = new Color(0, 0, 0, 0 - i);
            }
            alertQueue.Dequeue();
        }

    }

    
}
