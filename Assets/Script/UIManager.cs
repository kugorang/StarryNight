using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public Text goldDisplayer;
    //public Text goldDisplayerR;
    public Text itemLimitDisplayer;
    // public Text itemLimitDisplayerR;
    public GameObject displayPanel;

    public GameObject newBookAlert;
    public GameObject newItemAlert;
    public GameObject newUpgradeAlert;

    DataController dataController;

    private void Start()
    {
        dataController = DataController.Instance;    
    }

    void Update ()//널 체크를 통해 컴포넌트에서 필요한 일부만 쓸 수 있게 만든다. ex)골드 표시만 필요하면 goldDisplayer만 설정하고 나머지는 none으로 두면 됨.
    {
        if(SceneManager.GetActiveScene().name=="Main")
        {
            if (displayPanel!=null&&!displayPanel.activeSelf)//스스로의 활성화 여부 확인. 이 패널의 부모나 조상이 비활성화되면 오류 생김.
            {
                displayPanel.SetActive(true);
            }
            // 보유 골드량 표시
            if (goldDisplayer != null)
            {
                goldDisplayer.text = dataController.Gold + " 골드";
            }

            // 현재 아이템 보유량 및 최대 아이템 보유 가능량 표시
            if (itemLimitDisplayer != null)
            {
                itemLimitDisplayer.text = dataController.ItemCount + " / " + dataController.ItemLimit;
            }
        }
        else
        {
            displayPanel.SetActive(false);
        }
        if (newBookAlert!=null)
        { 
        newBookAlert.SetActive(dataController.newBookList.Count > 0);//deactive object when if count<=0 
        }
        if (newItemAlert != null)
        {
            newItemAlert.SetActive(dataController.newItemList.Count > 0);
        }
        if (newUpgradeAlert!= null)
        {
            newUpgradeAlert.SetActive(dataController.NewUpgrade);
        }
    }
}
