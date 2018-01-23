using UnityEngine;
using UnityEngine.UI;

public class ItemListManager : MonoBehaviour
{
    int starIdxStart, materialIdxStart, combineIdxStart;
    int starIdxMax, materialIdxMax, combineIdxMax;

    DataDictionary dataDic;

    public GameObject panelPrefab, itemInfoPanel;

    Transform starContentPanel, materialContentPanel, combineContentPanel;

    private static ItemListManager instance;

    public static ItemListManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<ItemListManager>();

            if (instance == null)
            {
                GameObject container = new GameObject("ItemListManager");
                instance = container.AddComponent<ItemListManager>();
            }
        }

        return instance;
    }

    private void Awake()
    {
        dataDic = DataController.Instance.GetComponent<DataDictionary>();

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
        for (int idx = starIdxStart + 1; idx <= starIdxMax; idx++)
        {
            AddItemButton(idx, starContentPanel);
        }

        for (int idx = materialIdxStart + 1; idx <= materialIdxMax; idx++)
        {
            AddItemButton(idx, materialContentPanel);
        }

        for (int idx = combineIdxStart + 1; idx <= combineIdxMax; idx++)
        {
            AddItemButton(idx, combineContentPanel);
        }
    }

    void AddItemButton(int idx, Transform tf)
    {
        GameObject itemListPanel = Instantiate(panelPrefab);
        Button itemBtn = itemListPanel.GetComponentInChildren<Button>();
        Image itemLock = itemListPanel.transform.Find("ItemLock").GetComponent<Image>();

        ItemInfo findItemInfo = dataDic.FindDic[idx];

        itemListPanel.transform.SetParent(tf);
        itemBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(findItemInfo.ImagePath);

        if (DataController.Instance.itemOpenList.Contains(idx))
        {
            itemLock.gameObject.SetActive(false);

            ColorBlock btnColors = itemBtn.colors;

            btnColors.normalColor = Color.white;
            btnColors.highlightedColor = Color.white;
            btnColors.pressedColor = Color.white;

            itemBtn.colors = btnColors;

            itemBtn.onClick.AddListener(() => ShowWindow(findItemInfo));
        }
    }

    public void ShowWindow(ItemInfo itemInfo)
    {
        itemInfoPanel.SetActive(true);

        ItemInfoWindow infoWindow = itemInfoPanel.transform.Find("ItemInfoWindow").GetComponent<ItemInfoWindow>();

        infoWindow.gameObject.SetActive(true);

        infoWindow.ItemImg.sprite = Resources.Load<Sprite>(itemInfo.ImagePath);
        infoWindow.ItemName.text = itemInfo.MtName;
        infoWindow.ItemSort.text = itemInfo.Group;
        infoWindow.ItemGrade.text = itemInfo.Grade;
        infoWindow.ItemCost.text = "판매 가격 : " + itemInfo.SellPrice.ToString();
        infoWindow.ItemText.text = itemInfo.Description;
    }
}