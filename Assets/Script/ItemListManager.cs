using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class ItemListManager : MonoBehaviour
    {
        private static ItemListManager _instance;

        private DataController _dataController;

        private DataDictionary _dataDic;
        public Sprite NewItemAlert;

        public GameObject PanelPrefab, ItemInfoPanel;

        private Transform _starContentPanel, _materialContentPanel, _combineContentPanel;
        private int _starIdxMax, _materialIdxMax, _combineIdxMax;
        private int _starIdxStart, _materialIdxStart, _combineIdxStart;

        public static ItemListManager GetInstance()
        {
            if (_instance != null) 
                return _instance;
            
            _instance = FindObjectOfType<ItemListManager>();

            if (_instance != null) 
                return _instance;
            
            var container = new GameObject("ItemListManager");
            _instance = container.AddComponent<ItemListManager>();

            return _instance;
        }

        private void Awake()
        {
            _dataController = DataController.Instance;
            _dataDic = DataDictionary.Instance;

            _starIdxStart = 1000;
            _materialIdxStart = 2000;
            _combineIdxStart = 3000;

            _starIdxMax = _starIdxStart + _dataDic.StarNum;
            _materialIdxMax = _materialIdxStart + _dataDic.MaterialNum;
            _combineIdxMax = _combineIdxStart + _dataDic.CombineNum;

            _starContentPanel = GameObject.Find("StarContentPanel").transform;
            _materialContentPanel = GameObject.Find("MaterialContentPanel").transform;
            _combineContentPanel = GameObject.Find("CombineContentPanel").transform;
        }

        private void Start()
        {
            for (var idx = _starIdxStart + 1; idx <= _starIdxMax; idx++) 
                AddItemButton(idx, _starContentPanel);

            for (var idx = _materialIdxStart + 1; idx <= _materialIdxMax; idx++) 
                AddItemButton(idx, _materialContentPanel);

            for (var idx = _combineIdxStart + 1; idx <= _combineIdxMax; idx++) 
                AddItemButton(idx, _combineContentPanel);
        }

        private void AddItemButton(int idx, Transform tf)
        {
            var itemListPanel = Instantiate(PanelPrefab);
            var itemBtn = itemListPanel.GetComponentInChildren<Button>();
            var itemLock = itemListPanel.transform.Find("ItemLock").GetComponent<Image>();

            var findItemInfo = _dataDic.FindItemDic[idx];

            itemListPanel.transform.SetParent(tf);
            itemBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(findItemInfo.ImagePath);

            if (!_dataController.ItemOpenList.Contains(idx)) 
                return;
            
            var btnColors = itemBtn.colors;

            btnColors.normalColor = Color.white;
            btnColors.highlightedColor = Color.white;
            btnColors.pressedColor = Color.white;

            itemBtn.colors = btnColors;
            itemBtn.onClick.AddListener(() => ShowWindow(findItemInfo));
            
            if (_dataController.NewItemList.Contains(idx))
            {
                // 새 아이템이면 느낌표 표시 추가
                itemLock.sprite = NewItemAlert;
                itemLock.raycastTarget = false;
                itemBtn.onClick.AddListener(() => RemoveAlert(idx, itemLock));
            }
            else
            {
                itemLock.gameObject.SetActive(false);
            }
        }

        private void ShowWindow(ItemInfo itemInfo)
        {
            ItemInfoPanel.SetActive(true);

            var infoWindow = ItemInfoPanel.transform.Find("ItemInfoWindow").GetComponent<ItemInfoWindow>();

            infoWindow.gameObject.SetActive(true);

            infoWindow.ItemImg.sprite = Resources.Load<Sprite>(itemInfo.ImagePath);
            infoWindow.ItemName.text = itemInfo.Name;
            infoWindow.ItemSort.text = itemInfo.Group;
            infoWindow.ItemGrade.text = itemInfo.Grade;
            infoWindow.ItemCost.text = "판매 가격 : " + itemInfo.SellPrice;
            infoWindow.ItemText.text = itemInfo.Description;
        }

        private void RemoveAlert(int idx, Component lockImg)
        {
            // 더티 플래그와 느낌표 표시 갱신
            _dataController.NewItemList.Remove(idx);
            DataController.SaveGameData(_dataController.NewItemList, _dataController.NewItemListPath);
            lockImg.gameObject.SetActive(false);
        }
    }
}