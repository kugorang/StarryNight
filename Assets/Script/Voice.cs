using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class Voice : MonoBehaviour
    {
        public Button Character;

        public void OnClick()
        {
            AudioManager.GetInstance().VoiceSound();
            StartCoroutine(WaitVoice());
        }

        // 대사 간 간격 시간 설정
        private IEnumerator WaitVoice()
        {
            Character.enabled = false;
            yield return new WaitForSeconds(2.0f);
            Character.enabled = true;
        }
    }
}