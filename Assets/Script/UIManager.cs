using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text goldDisplayer;
    public Text itemLimitDisplayer;

	void Update ()
    {
        // 보유 골드량 표시
        goldDisplayer.text = DataController.Instance.Gold + " 원";
        // 현재 아이템 보유량 및 최대 아이템 보유 가능량 표시
        itemLimitDisplayer.text = DataController.Instance.ItemCount + " / " + DataController.Instance.ItemLimit;
    }
}
