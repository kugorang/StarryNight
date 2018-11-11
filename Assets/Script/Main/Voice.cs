#region

using System.Collections;
using Script.Common;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Script.Main
{
    public class Voice : MonoBehaviour
    {
        public Button Character;

        public void OnClick()
        {
            AudioManager.Instance.VoiceSound();
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