using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script
{
    public class UIManager : MonoBehaviour
    {
        private DataController _dataController;

        // public Text itemLimitDisplayerR;
        public GameObject DisplayPanel;

        public Text GoldDisplayer;

        //public Text goldDisplayerR;
        public Text ItemLimitDisplayer;

        public GameObject NewBookAlert;
        public GameObject NewItemAlert;
        public GameObject NewUpgradeAlert;

        private void Start()
        {
            _dataController = DataController.Instance;
        }

        // NULL 체크를 통해 컴포넌트에서 필요한 일부만 쓸 수 있게 만든다.
        // ex) 골드 표시만 필요하면 goldDisplayer만 설정하고 나머지는 none으로 두면 됨.
        private void Update() 
        {
            if (SceneManager.GetActiveScene().name == "Main")
            {
                // 스스로의 활성화 여부 확인. 이 패널의 부모나 조상이 비활성화되면 오류 생김.
                if (DisplayPanel != null && !DisplayPanel.activeSelf) 
                    DisplayPanel.SetActive(true);
                
                // 보유 골드량 표시
                if (GoldDisplayer != null) 
                    GoldDisplayer.text = _dataController.Gold + " 골드";

                // 현재 아이템 보유량 및 최대 아이템 보유 가능량 표시
                if (ItemLimitDisplayer != null)
                    ItemLimitDisplayer.text = _dataController.ItemCount + " / " + _dataController.ItemLimit;
            }
            else
            {
                DisplayPanel.SetActive(false);
            }

            if (NewBookAlert != null)
                // deactive object when if count<=0
                NewBookAlert.SetActive(_dataController.NewBookList.Count > 0); 
            
            if (NewItemAlert != null) 
                NewItemAlert.SetActive(_dataController.NewItemList.Count > 0);
            
            if (NewUpgradeAlert != null) 
                NewUpgradeAlert.SetActive(_dataController.NewUpgrade);
        }
    }
}