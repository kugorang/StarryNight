using UnityEngine;

namespace Script
{
    public class UpgradePopup : MonoBehaviour
    {
        private GameObject _upgrade;

        private void Awake()
        {
            _upgrade = GameObject.Find("Upgrade Panel");
            _upgrade.SetActive(false);
        }

        // 업그레이드 팝업 띄우기
        public void EnterUpgrade()
        {
            transform.SetAsLastSibling();
            _upgrade.SetActive(true);
        }

        // 업그레이드 팝업 닫기
        public void ExitUpgrade()
        {
            _upgrade.SetActive(false);
        }
    }
}