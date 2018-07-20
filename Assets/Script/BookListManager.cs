using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Script
{
    public class BookListManager : MonoBehaviour
    {
        private static BookListManager _instance;
        private DataController _dataController;
        private DataDictionary _dataDic;
        public Sprite NewItemAlert;
        public GameObject PanelPrefab, ItemInfoPanel, ExchangePanel; // Exchange는 기본적으로 비활성화 상태
        // 1번째 원소는 1단계 아이템 교환 개수, ... 5번째 원소는 5단계 아이템 교환 개수
        public int[] ExchangeRatio = {10, 8, 6, 4, 2};

        private Transform _setContentPanel;
        private int _setIdxStart, _setIdxMax;

        public int Cols = 5;//열 수

        public static BookListManager Instance
        {
            get
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
        }

        private void Awake()
        {
            _dataController = DataController.Instance;

            /*GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();*/
            _dataDic = DataController.Instance.GetComponent<DataDictionary>();
            _setContentPanel = GameObject.Find("SetContentPanel").transform;
        }

        private void Start()
        {
          
            foreach (var setItemInfo in _dataDic.SetCombineList)
            {
                AddItemButton(setItemInfo.Index1, _setContentPanel);
                AddItemButton(setItemInfo.Index2, _setContentPanel);
                AddItemButton(setItemInfo.Index3, _setContentPanel);
                AddItemButton(setItemInfo.Index4, _setContentPanel);
                AddItemButton(setItemInfo.Result, _setContentPanel);
            }
        }
        
        /// <summary>
        /// 아이템의 인덱스를 받아 그 아이템에 대한 버튼을 만들어주는 함수
        /// </summary>
        /// <param name="idx">아이템 인덱스</param>
        /// <param name="tf">버튼이 속할 UI 패널</param>
        private void AddItemButton(int idx, Transform tf)
        {
            var itemListPanel = Instantiate(PanelPrefab);
            var itemBtn = itemListPanel.transform.Find("ItemListButton").GetComponent<Button>();
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
            // 안 열렸으면 교환 기능 사용 가능
            else
            {
                var isResultItem = false;
                
                // 조합된 서적 아이템인지 확인
                foreach (var setItemInfo in _dataDic.SetCombineList)
                {
                    isResultItem = isResultItem || (setItemInfo.Result == idx);
                }

                // 서적이 아니어야 교환기능 사용
                if (!isResultItem) 
                {
                    itemBtn.onClick.AddListener(() => ExchangeItem(idx));
                }
            }

            if (_dataController.NewBookList.Contains(idx))
            {
                // 새 아이템이면 느낌표 표시
                itemLock.sprite = NewItemAlert;
                itemLock.raycastTarget = false;
                itemBtn.onClick.AddListener(() => RemoveAlert(idx, itemLock));
            }
            else
            {
                itemLock.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 아이템의 정보를 창에 띄워 표시해줌
        /// </summary>
        /// <param name="itemInfo">아이템의 정보</param>
        private void ShowWindow(ItemInfo itemInfo)
        {
            ItemInfoPanel.SetActive(true);

            var infoWindow = ItemInfoPanel.transform.Find("ItemInfoWindow").GetComponent<ItemInfoWindow>();

            infoWindow.gameObject.SetActive(true);

            infoWindow.ItemImg.sprite = Resources.Load<Sprite>(itemInfo.ImagePath);
            infoWindow.ItemName.text = itemInfo.Name;
            infoWindow.ItemSort.text = itemInfo.Group;
            /*infoWindow.ItemGrade.text = itemInfo.Grade;*/
            infoWindow.ItemCost.text = "획득 보상 : " + itemInfo.SellPrice + " 골드";
            infoWindow.ItemText.text = itemInfo.Description;
        }
        
        /// <summary>
        /// 새 아이템 더티 플래그를 제거해줌
        /// </summary>
        /// <param name="idx">아이템 인덱스</param>
        /// <param name="lockImg">자물쇠모양 이미지 겸 느낌표 이미지</param>
        private void RemoveAlert(int idx, Component lockImg)
        {
            // 획득했으므로 더티 플래그와 느낌표 아이콘 갱신
            _dataController.NewBookList.Remove(idx);
            DataController.SaveGameData(_dataController.NewBookList, _dataController.NewBookListPath);
            lockImg.gameObject.SetActive(false);

            // 아이템 정보를 얻어 획득 보상 처리 (최초 1회)
            var item = _dataDic.FindItemDic[idx];
            PopUpWindow.Alert(item.Name + " 획득 보상: " + item.SellPrice + " 골드");
            _dataController.Gold += (ulong)item.SellPrice;

            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in _dataController.Observers)
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));
        }

        private void ExchangeItem(int idx)
        {
            ExchangePanel.transform.Find("Text").GetComponent<Text>().text = _dataDic.FindItemDic[idx].Name + " 교환";
            
            for (var i = 1; i <= 5; i++)
            {
                // 혹시 있을 Closure 오류 방지
                var btnIdx = i - 1;
                ExchangePanel.transform.Find("Button" + i).GetComponent<Button>()
                    .onClick.AddListener(() => ExchangeBtnClick(idx,btnIdx));
            }
            
            ExchangePanel.SetActive(true);
        }

        private void ExchangeBtnClick(int itemIdx, int btnIdx)
        {            
            int[] deleteAmount=new int[3];//[0]Red [1]Blue [2]Green 삭제될 별 개수
            for (var i = 0; i < 3; i++)
            {
                // 별 아이템의 index
                var starIdx = 1001 + btnIdx  + i*5;
                deleteAmount[i] = _dataController.GetItemNum(starIdx);
            }
            
            // 별 아이템 개수가 필요한 양(ExchangeRatio[btnIdx])보다 많거나 같을 경우
            if (deleteAmount.Sum() >= ExchangeRatio[btnIdx])
            {
                for (var i = 0; i < 3; i++)
                {
                    // 별 아이템의 index
                    var starIdx = 1001 + btnIdx + i * 5;
                    for (var j = 0; j < deleteAmount[i]; j++)
                    {
                        _dataController.DeleteItem(starIdx);
                    }
                }

                _dataController.InsertNewItem(itemIdx);
                ExchangePanel.SetActive(false);
                
                // refresh scene
                SceneManager.LoadScene("BookList");
            }
            else
            {
                // TODO: 잠깐 버튼 클릭 막기
                PopUpWindow.Alert("재료가 부족합니다.");
            }
        }
    }
}