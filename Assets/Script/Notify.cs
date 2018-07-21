using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notify : MonoBehaviour
{
    private static Text _notifyText;
    private static RectTransform _rtf;
    private static bool _isMinimized=false;
    private static bool _enabled=true;
    private static GameObject _gameObj;
    private static Image _btnImage;
    private const int ReferenceW = 1080;

    private static string _notification="";

    // Use this for initialization
    void Awake()//Notify는 Dialog에 있어 매 씬 이동마다 파괴후 재 생성된다.
    {
       Initialize();
        if (_notification == "")//씬 전환 전에도 빈 문자열이면 숨긴다.
        {
            Text = "";
        }
    }

    private void Initialize()
    {
        _gameObj = GameObject.Find("Notification Displayer");
        if (_gameObj == null)
        {
            Debug.LogWarning("Notification object is destroyed and not able to notify.");
            return;
        }

        _notifyText = _gameObj.GetComponentInChildren<Text>();
        _rtf = _gameObj.GetComponent<RectTransform>();
        _btnImage = _gameObj.GetComponent<Image>();
    }
    /// <summary>
    /// 창의 크기와 위치를 알맞게 변경.
    /// </summary>
    private static void Refresh()
    {
        if (_enabled)
        {
            _btnImage.raycastTarget = true;
            _btnImage.color = new Color(0f, 0f, 0f, 0.5f);
            _notifyText.color = new Color(1, 1, 1, 1);
        }
        else
        {
            _btnImage.raycastTarget = false;
            _btnImage.color = new Color(0.5f, 0.5f, 0.5f, 0);
            _notifyText.color = new Color(1, 1, 1, 0);
        }
        if (_isMinimized) //TODO: 해상도에 따라 다르게 지원할 것.
        {

            _rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
            _rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
            _rtf.anchoredPosition3D = new Vector3((ReferenceW / 2) - 60, -220, 0);//현재 Canvas Scaler설정 탓에 Screen.Width를 쓸 필요가 없음.
            _notifyText.text = "?";
        }
        else
        {
            var height = (_notification.Length / 15) * 50 + 100;//한 줄 당 약 15글자, 줄 하나당 추가로 높이 약 50 필요. 3줄 이상(띄어쓰기 포함 45 자 이상)은 필요 없을 거라 추정함.
            _rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ReferenceW);
            _rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _rtf.anchoredPosition3D = new Vector3(0, -160 - height / 2, 0);//-160은 Displayer 패널의 끝, 따라서 중심은 그보다 height/2만큼.
            _notifyText.text = _notification;
        }
    }

    /// <summary>
    /// 최소화 상태와 최대화 상태에 따라 크기 전환.
    /// </summary>
    public void Toggle()//버튼으로 호출할 수 있어야해서 static이 아님
    {
        _isMinimized = !_isMinimized;
        Refresh();
        
    }

    /// <summary>
    /// 알림창에 띄울 내용을 할당합니다. 빈 문자열을 할당하면, 알림창이 자동으로 비활성화 됩니다.
    /// </summary>
    public static string Text
    {
    get { return _notification; }
    set
    {
        _notification = value;
        _notifyText.text = _notification;
        _enabled=(_notification != "");//빈 문자열이 아니면 활성화, 빈 문자열이면 비활성화
        if (_isMinimized)
        {
            _gameObj.GetComponent<Notify>().Toggle();
        }
        else //Toggle엔 Refresh가 포함되어 있다.
        {
            Refresh();
        }

    }
}
    private void OnEnable()                         // Awake 이후 재 검증
    {
        if (_gameObj == null)
            Initialize();
    }

   

    private void OnDisable()
    {
        _gameObj = null;
        _rtf = null;
        _notifyText = null;
        _btnImage = null;
    }
}
