using Script;
using UnityEngine;
using UnityEngine.UI;

public class ItemListManager : MonoBehaviour
{
    private static ItemListManager instance;

    private DataController dataController;

    private DataDictionary dataDic;
    public Sprite NewItemAlert;

    public GameObject panelPrefab, itemInfoPanel;

    private Transform starContentPanel, materialContentPanel, combineContentPanel;
    private int starIdxMax, materialIdxMax, combineIdxMax;
    private int starIdxStart, materialIdxStart, combineIdxStart;

    public static ItemListManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<ItemListManager>();

            if (instance == null)
            {
                var container = new GameObject("ItemListManager");
                instance = container.AddComponent<ItemListManager>();
            }
        }

        return instance;
    }

    private void Awake()
    {
        dataController = DataController.Instance;
        dataDic = DataDictionary.Instance;

        starIdxStart = 1000;
        materialIdxStart = 2000;
        combineIdxStart = 3000;

        starIdxMax = starIdxStart + dataDic.StarNum;
        materialIdxMax = materialIdxStart + dataDic.MaterialNum;
        combineIdxMax = combineIdxStart + dataDic.CombineNum;

        starContentPanel = GameObject.Find("StarContentPanel").transform;
        materialContentPanel = GameObject.Find("MaterialContentPanel").transform;
        combineContentPanel = GameObject.Find("CombineContentPanel").transform;
    }

    private void Start()
    {
        for (var idx = starIdxStart + 1; idx <= starIdxMax; idx++) AddItemButton(idx, starContentPanel);

        for (var idx = materialIdxStart + 1; idx <= materialIdxMax; idx++) AddItemButton(idx, materialContentPanel);

        for (var idx = combineIdxStart + 1; idx <= combineIdxMax; idx++) AddItemButton(idx, combineContentPanel);
    }

    private void AddItemButton(int idx, Transform tf)
    {
        var itemListPanel = Instantiate(panelPrefab);
        var itemBtn = itemListPanel.GetComponentInChildren<Button>();
        var itemLock = itemListPanel.transform.Find("ItemLock").GetComponent<Image>();

        var findItemInfo = dataDic.FindItemDic[idx];

        itemListPanel.transform.SetParent(tf);
        itemBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(findItemInfo.ImagePath);

        if (dataController.ItemOpenList.Contains(idx))
        {
            var btnColors = itemBtn.colors;

            btnColors.normalColor = Color.white;
            btnColors.highlightedColor = Color.white;
            btnColors.pressedColor = Color.white;

            itemBtn.colors = btnColors;
            itemBtn.onClick.AddListener(() => ShowWindow(findItemInfo));
            if (dataController.NewItemList.Contains(idx))
            {
                //새 아이템이면 느낌표 표시 추가
                itemLock.sprite = NewItemAlert;
                itemLock.raycastTarget = false;
                itemBtn.onClick.AddListener(() => RemoveAlert(idx, itemLock));
            }
            else
            {
                itemLock.gameObject.SetActive(false);
            }
        }
    }

    public void ShowWindow(ItemInfo itemInfo)
    {
        itemInfoPanel.SetActive(true);

        var infoWindow = itemInfoPanel.transform.Find("ItemInfoWindow").GetComponent<ItemInfoWindow>();

        infoWindow.gameObject.SetActive(true);

        infoWindow.ItemImg.sprite = Resources.Load<Sprite>(itemInfo.ImagePath);
        infoWindow.ItemName.text = itemInfo.Name;
        infoWindow.ItemSort.text = itemInfo.Group;
        infoWindow.ItemGrade.text = itemInfo.Grade;
        infoWindow.ItemCost.text = "판매 가격 : " + itemInfo.SellPrice;
        infoWindow.ItemText.text = itemInfo.Description;
    }

    public void RemoveAlert(int idx, Image lockImg)
    {
        //더티 플래그와 느낌표 표시 갱신
        dataController.NewItemList.Remove(idx);
        DataController.SaveGameData(dataController.NewItemList, dataController.NewItemListPath);
        lockImg.gameObject.SetActive(false);
    }
}