using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text goldDisplayerL;
    public Text goldDisplayerR;
    public Text itemLimitDisplayerL;
    public Text itemLimitDisplayerR;

    public GameObject newBookAlert;
    public GameObject newItemAlert;
    public GameObject newUpgradeAlert;

    DataController dataController;

    private void Start()
    {
        dataController = DataController.Instance;    
    }

    void Update ()
    {
        // 보유 골드량 표시
        goldDisplayerL.text = dataController.Gold + " 원";
        goldDisplayerR.text = goldDisplayerL.text;
        // 현재 아이템 보유량 및 최대 아이템 보유 가능량 표시
        itemLimitDisplayerL.text = dataController.ItemCount + " / " + dataController.ItemLimit;
        itemLimitDisplayerR.text = itemLimitDisplayerL.text;
        newBookAlert.SetActive(dataController.newBookList.Count > 0);//deactive object when if count<=0 
        newItemAlert.SetActive(dataController.newItemList.Count > 0);
        newUpgradeAlert.SetActive(dataController.NewUpgrade);
    }
}
