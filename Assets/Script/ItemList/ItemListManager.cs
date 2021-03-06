﻿#region

using Script.Common;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Script.ItemList
{
    public class ItemListManager : MonoBehaviour
    {
        private static ItemListManager _instance;
        private DataController _dataController;
        private DataDictionary _dataDic;
        private RectTransform _starContentPanel, _materialContentPanel, _combineContentPanel, _etcContentPanel, _contentPanel;

        private int _starIdxMax, _materialIdxMax, _combineIdxMax, _etcIdxMax;
        private int _starIdxStart, _materialIdxStart, _combineIdxStart, _etcIdxStart;
        public Sprite NewItemAlert;

        public GameObject PanelPrefab, ItemInfoPanel;

        public static ItemListManager Instance
        {
            get
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
        }

        private void Awake()
        {
            _dataController = DataController.Instance;
            _dataDic = DataDictionary.Instance;

            _starIdxStart = 1000;
            _materialIdxStart = 2000;
            _combineIdxStart = 3000;
            _etcIdxStart = 5000;

            _starIdxMax = _starIdxStart + _dataDic.StarNum;
            _materialIdxMax = _materialIdxStart + _dataDic.MaterialNum;
            _combineIdxMax = _combineIdxStart + _dataDic.CombineNum;
            _etcIdxMax = _etcIdxStart + _dataDic.EtcNum;

            _starContentPanel = GameObject.Find("StarContentPanel").GetComponent<RectTransform>();
            _materialContentPanel = GameObject.Find("MaterialContentPanel").GetComponent<RectTransform>();
            _combineContentPanel = GameObject.Find("CombineContentPanel").GetComponent<RectTransform>();
            _etcContentPanel = GameObject.Find("EtcContentPanel").GetComponent<RectTransform>();
            _contentPanel = GameObject.Find("Content").GetComponent<RectTransform>();
        }

        private void Start()
        {
            for (var idx = _starIdxStart + 1; idx <= _starIdxMax; idx++)
                AddItemButton(idx, _starContentPanel);

            /*SetDynamicGrid(_starContentPanel, _starContentPanel.gameObject.GetComponent<GridLayoutGroup>(), _dataDic.StarNum, 5, 35);*/

            for (var idx = _materialIdxStart + 1; idx <= _materialIdxMax; idx++)
                AddItemButton(idx, _materialContentPanel);

            for (var idx = _combineIdxStart + 1; idx <= _combineIdxMax; idx++)
                AddItemButton(idx, _combineContentPanel);

            for (var idx = _etcIdxStart + 1; idx <= _etcIdxMax; idx++)
                AddItemButton(idx, _etcContentPanel);
        }

        private void AddItemButton(int idx, Transform tf)
        {
            var itemListPanel = Instantiate(PanelPrefab);
            var itemBtn = itemListPanel.transform.Find("ItemListButton").GetComponent<Button>();
            var itemLock = itemListPanel.transform.Find("ItemLock").GetComponent<Image>();

            var findItemInfo = _dataDic.FindItemDic[idx];

            itemListPanel.transform.SetParent(tf);
            itemBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(findItemInfo.ImagePath);

            if (!_dataController.ItemOpenList.Contains(idx)) return;

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

        /*// cnt : 총 cell 갯수, minColsInARow : 한 행에 최소 cell 갯수, maxRow : 최대 행 수.
        private void SetDynamicGrid(RectTransform targetCanvas, GridLayoutGroup targetGrid, int cnt, int minColsInARow,
            int maxRow)
        {
            /*var originWidth = targetCanvas.rect.width;
            var originHeight = targetCanvas.rect.height;
            
            var rows = Mathf.Clamp ( Mathf.CeilToInt((float) cnt / minColsInARow), 1, maxRow + 1);
            var cols = Mathf.CeilToInt ((float)cnt / rows);

            var spaceW = (targetGrid.padding.left + targetGrid.padding.right) + (targetGrid.spacing.x * (cols - 1));
            var spaceH = (targetGrid.padding.top + targetGrid.padding.bottom) + (targetGrid.spacing.y * (rows - 1));

            var maxWidth = originWidth - spaceW;
            var maxHeight = originHeight - spaceH;

            var width =  Mathf.Min(targetCanvas.rect.width - (targetGrid.padding.left + targetGrid.padding.right) - (targetGrid.spacing.x * (cols-1)) , maxWidth);
            var height = Mathf.Min(targetCanvas.rect.height - (targetGrid.padding.top + targetGrid.padding.bottom) - (targetGrid.spacing.y * (rows-1)), maxHeight);

            targetGrid.cellSize = new Vector2 (width / cols, height / rows);#1#

            Debug.Log("Screen.width : " + Screen.width);
            Debug.Log("_contentPanel.rect.width : " + _contentPanel.rect.width);

            targetGrid.cellSize = new Vector2(Screen.width * 0.18518f, Screen.width * 0.18518f);
            targetGrid.spacing = new Vector2(Screen.width * 0.01389f, Screen.width * 0.01389f);
        }

        private static float PxtoDp(float px)
        {
            return px * 160 / Screen.dpi;
        }*/

        private void ShowWindow(ItemInfo itemInfo)
        {
            ItemInfoPanel.SetActive(true);

            var infoWindow = ItemInfoPanel.transform.Find("ItemInfoWindow").GetComponent<ItemInfoWindow>();

            infoWindow.gameObject.SetActive(true);

            infoWindow.ItemImg.sprite = Resources.Load<Sprite>(itemInfo.ImagePath);
            infoWindow.ItemName.text = itemInfo.Name;
            infoWindow.ItemSort.text = itemInfo.Group;
            infoWindow.ItemGrade.text = string.Format("{0}레벨 아이템", itemInfo.Grade);
            infoWindow.ItemCost.text = string.Format("판매 가격 : {0}", itemInfo.SellPrice);
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