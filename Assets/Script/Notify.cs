using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notify : MonoBehaviour
{
    private static Text _notifyText;
    private static RectTransform _rtf;
    private static bool _isMinimized;
    private static GameObject _gameObj;
    private static Image _btnImage;

    private static string _notification;

    // Use this for initialization
    void Awake()
    {
       Initialize();
        _isMinimized = false;
        Text = "";
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
    /// 최소화 상태와 최대화 상태에 따라 크기 전환.
    /// </summary>
    public void Toggle()
    {
        _isMinimized = !_isMinimized;
        if (_isMinimized) //TODO: 해상도에 따라 다르게 지원할 것.
        {
            _rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
            _rtf.anchoredPosition3D = new Vector3((Screen.width / 2) - 60, 740, 0);
            _notifyText.text = "?";
        }
        else
        {
            _rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            _rtf.anchoredPosition3D = new Vector3(0, 750, 0);
            _notifyText.text = _notification;
        }
    }
    /// <summary>
    /// 버튼을 활성화/비활성화함. 실제로는 투명화함.
    /// </summary>
    /// <param name="enabled">True면 활성화</param>
    private void Enable(bool enabled)
    {
        if (enabled)
        {
            if (_isMinimized)
            {
                Toggle();
            }

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
        _gameObj.GetComponent<Notify>().Enable(_notification != "");//빈 문자열이 아니면 활성화, 빈 문자열이면 비활성화
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
