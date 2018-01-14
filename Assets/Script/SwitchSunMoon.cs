using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSunMoon : MonoBehaviour {

    public UnityEngine.UI.Button SunMoonbtn;
    public int state;
    public Sprite sun;
    public Sprite moon;

    private static SwitchSunMoon instance;

    public static SwitchSunMoon GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<SwitchSunMoon>();

            if (instance == null)
            {
                GameObject container = new GameObject("SwitchSunMoon");

                instance = container.AddComponent<SwitchSunMoon>();
            }
        }
        return instance;
    }



    void Start()
    {
        if (SunMoonbtn == null)
            SunMoonbtn = gameObject.GetComponent<UnityEngine.UI.Button>();
    }

    void Update()
    {
        if (state == 1)
            gameObject.GetComponent<Image>().sprite = sun;
        else
            gameObject.GetComponent<Image>().sprite = moon;

    }

    // 현재 스위치 상태 가져오기
    public int GetState()
    {
        return state;
    }

    // 버튼 스위치
    public void CheckButton()
    {
        if (state == 1) // 1이 sun 2가 moon
            state = 0;
        else
            state = 1;
    }
}
