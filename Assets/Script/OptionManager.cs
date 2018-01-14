using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour {

    public Button bgm;
    public Button effect;
    public Button voice;

    public Text bgmDisplayer;
    public Text effDisplayer;
    public Text voiceDisplayer;

    private void Awake()
    {
        // 배경음악 On Off 버튼 이미지와 텍스트 설정
        if (AudioManager.GetInstance().GetBGMAlive() == 1)
        {
            bgm.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
            bgmDisplayer.text = "ON";
        }
        else
        {
            AudioManager.GetInstance().BGMOff();
            bgm.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
            bgmDisplayer.text = "OFF";
        }

        // 효과음 On Off 버튼 이미지와 텍스트 설정
        if (AudioManager.GetInstance().GetEffAlive() == 1)
        {
            effect.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
            effDisplayer.text = "ON";
        }
        else
        {
            effect.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
            effDisplayer.text = "OFF";
        }

        // 캐릭터 대사 On Off 버튼 이미지와 텍스트 설정
        if (AudioManager.GetInstance().GetVoiceAlive() == 1)
        {
            voice.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
            voiceDisplayer.text = "ON";
        }
        else
        {
            voice.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
            voiceDisplayer.text = "OFF";
        }
    }

    // 배경음악 버튼 선택 시
    public void BGMButton()
    {
        if (AudioManager.GetInstance().GetBGMAlive() == 1) // 음악이 켜져있다면
        {
            AudioManager.GetInstance().BGMOff();
            AudioManager.GetInstance().SetBGMAlive(0);
            bgm.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
            bgmDisplayer.text = "OFF";
        }
        else // 음악이 꺼져있다면
        {
            AudioManager.GetInstance().BGMOn();
            AudioManager.GetInstance().SetBGMAlive(1);
            bgm.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
            bgmDisplayer.text = "ON";
        }
    }

    // 효과음 버튼 선택 시
    public void EffectButton()
    {
        if (AudioManager.GetInstance().GetEffAlive() == 1) // 음악이 켜져있다면
        {
            AudioManager.GetInstance().SetEffAlive(0);
            effect.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
            effDisplayer.text = "OFF";
        }
        else // 음악이 꺼져있다면
        {
            AudioManager.GetInstance().SetEffAlive(1);
            effect.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
            effDisplayer.text = "ON";
        }
    }

    // 캐릭터 대사 버튼 선택 시
    public void VoiceButton()
    {
        if (AudioManager.GetInstance().GetVoiceAlive() == 1) // 켜져있다면
        {
            AudioManager.GetInstance().SetVoiceAlive(0);
            voice.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
            voiceDisplayer.text = "OFF";
        }
        else // 꺼져있다면
        {
            AudioManager.GetInstance().SetVoiceAlive(1);
            voice.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
            voiceDisplayer.text = "ON";
        }
    }

}
