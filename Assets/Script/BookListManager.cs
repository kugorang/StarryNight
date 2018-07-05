using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script
{
    public class BookListManager : MonoBehaviour
    {
        private static BookListManager _instance;
        private DataController _dataController;
        private DataDictionary _dataDic;
        public Sprite NewItemAlert;
        public GameObject PanelPrefab, ItemInfoPanel;
        private Transform _setContentPanel;
        private int _setIdxStart, _setIdxMax;

        public static BookListManager GetInstance()
        {
            if (_instance != null) 
                return _instance;
            
            _instance = FindObjectOfType<BookListManager>();

            if (_instance != null) 
                return _instance;
            
            var container = new GameObject("BookListManager");
            _instance = container.AddComponent<BookListManager>();

            return _instance;
        }

        private void Awake()
        {
            _dataController = DataController.Instance;

            GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
            _dataDic = DataController.Instance.GetComponent<DataDictionary>();
            _setContentPanel = GameObject.Find("SetContentPanel").transform;
        }

        private void Start()
        {
            var structNum = _dataDic.SetComineList.Count;

            for (var index = 0; index < structNum; index++)
            {
                var setItemInfo = _dataDic.SetComineList[index];

                AddItemButton(setItemInfo.Index1, _setContentPanel);
                AddItemButton(setItemInfo.Index2, _setContentPanel);
                AddItemButton(setItemInfo.Index3, _setContentPanel);
                AddItemButton(setItemInfo.Index4, _setContentPanel);
                AddItemButton(setItemInfo.Result, _setContentPanel);
            }
        }

        private void AddItemButton(int idx, Transform tf)
        {
            var itemListPanel = Instantiate(PanelPrefab);
            var itemBtn = itemListPanel.GetComponentInChildren<Button>();
            var itemLock = itemListPanel.transform.Find("ItemLock").GetComponent<Image>();

            var findItemInfo = _dataDic.FindItemDic[idx];

            itemListPanel.transform.SetParent(tf);
            itemBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(findItemInfo.ImagePath);

            if (DataController.Instance.ItemOpenList.Contains(idx))
            {
                var btnColors = itemBtn.colors;

                btnColors.normalColor = Color.white;
                btnColors.highlightedColor = Color.white;
                btnColors.pressedColor = Color.white;

                itemBtn.colors = btnColors;

                itemBtn.onClick.AddListener(() => ShowWindow(findItemInfo));
            }

            if (_dataController.NewBookList.Contains(idx))
            {
                //새 아이템이면 느낌표 표시
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
            infoWindow.ItemCost.text = "획득 보상 : " + itemInfo.SellPrice + " 골드";
            infoWindow.ItemText.text = itemInfo.Description;
        }

        private void RemoveAlert(int idx, Component lockImg)
        {
            // 획득했으므로 더티 플래그와 느낌표 아이콘 갱신
            _dataController.NewBookList.Remove(idx);
            DataController.SaveGameData(_dataController.NewBookList, _dataController.NewBookListPath);
            lockImg.gameObject.SetActive(false);
            
            // 아이템 정보를 얻어 획득 보상 처리(최초 1회)
            var item = _dataDic.FindItemDic[idx];
            PopUpWindow.Alert(item.Name + " 획득 보상: " + item.SellPrice + " 골드", this);
            _dataController.Gold += (ulong) item.SellPrice;

            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));
        }
    }
}