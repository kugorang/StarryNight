using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text goldDisplayerL;
    public Text goldDisplayerR;
    public Text itemLimitDisplayerL;
    public Text itemLimitDisplayerR;

	void Update ()
    {
        // 보유 골드량 표시
        goldDisplayerL.text = DataController.Instance.Gold + " 원";
        goldDisplayerR.text = goldDisplayerL.text;
        // 현재 아이템 보유량 및 최대 아이템 보유 가능량 표시
        itemLimitDisplayerL.text = DataController.Instance.ItemCount + " / " + DataController.Instance.ItemLimit;
        itemLimitDisplayerR.text = itemLimitDisplayerL.text;
    }
}
