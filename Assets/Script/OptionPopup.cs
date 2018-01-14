using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPopup : MonoBehaviour {

    private GameObject option;

    private void Awake()
    {
        option = GameObject.Find("Option Panel");
        option.SetActive(false);
    }

    // 설정 팝업 띄우기
    public void EnterOption()
    {
        AudioManager.GetInstance().OptionSound();
        option.SetActive(true);
    }

    // 설정 팝업 닫기
    public void ExitOption()
    {
        option.SetActive(false);
    }
}
