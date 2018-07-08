using UnityEngine;

namespace Script
{
    public class OptionPopup : MonoBehaviour
    {
        private GameObject _option;

        private void Awake()
        {
            _option = GameObject.Find("Option Panel");
            _option.SetActive(false);
        }

        // 설정 팝업 띄우기
        public void EnterOption()
        {
            AudioManager.Instance.OptionSound();
            _option.SetActive(true);
        }

        // 설정 팝업 닫기
        public void ExitOption()
        {
            _option.SetActive(false);
        }
    }
}