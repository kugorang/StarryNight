using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Voice : MonoBehaviour
{
    public Button character;

    public void OnClick()
    {
        AudioManager.GetInstance().VoiceSound();
        StartCoroutine(WaitVoice());
    }

    // 대사 간 간격 시간 설정
    IEnumerator WaitVoice()
    {
        character.enabled = false;
        yield return new WaitForSeconds(2.0f);
        character.enabled = true;
    }
}