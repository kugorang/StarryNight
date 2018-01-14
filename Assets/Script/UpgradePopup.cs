using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePopup : MonoBehaviour
{
    private GameObject upgrade;

    private void Awake()
    {
        upgrade = GameObject.Find("Upgrade Panel");
        upgrade.SetActive(false);
    }

    // 업그레이드 팝업 띄우기
    public void EnterUpgrade()
    {
        transform.SetAsLastSibling();
        upgrade.SetActive(true);
    }

    // 업그레이드 팝업 닫기
    public void ExitUpgrade()
    {
        upgrade.SetActive(false);
    }
}