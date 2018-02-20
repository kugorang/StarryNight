using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PopUpAlert : MonoBehaviour {

    private static bool isLocked;
    private static Queue alertQueue;

    [SerializeField]
    private static GameObject AlertPanel;
    private static Text AlertText;
    /*private static GameObject AlertPanelR;
    private static Text AlertTextR;*/
    // Use this for initialization
    void Awake () {
        if (AlertPanel == null)
        {
            AlertPanel = gameObject;
        }
        if (AlertText == null)
        {
            AlertText = AlertPanel.GetComponentInChildren<Text>();
        }
        /*if(AlertPanelR == null)
        {
            AlertPanelR = GameObject.Find("OnEventAlertR");
        }
        if (AlertTextR == null)
        {
            AlertTextR = AlertPanelR.GetComponentInChildren<Text>();
        }*/
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
        Debug.Log(alertQueue.Count + " count of queue!! "+alertQueue.Peek());
        if (alertQueue.Count <= 1)
        {
            GameObject alertPanel = AlertPanel;
            Text alertText = AlertText;
            Vector3 pos = alertPanel.transform.position;
            if (isRight&&pos.x<0)
            {
                /*   alertPanel = AlertPanelR;
                   alertText = AlertTextR;*/
                
                alertPanel.transform.position = new Vector3(540, pos.y, pos.z);
            }
            else if(!isRight&&pos.x>0)
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
            Debug.Log("Alert Pannel is Null Object.");
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

    private void OnDestroy()
    {
        AlertPanel = null;
        AlertText = null;
    }


}
