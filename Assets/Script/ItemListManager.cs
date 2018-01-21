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
        dataDic = DataController.GetInstance().GetComponent<DataDictionary>();

        starIdxStart = 1000;
        materialIdxStart = 2000;
        combineIdxStart = 3000;

        starIdxMax = starIdxStart + dataDic.starNum;
        materialIdxMax = materialIdxStart + dataDic.materialNum;
        combineIdxMax = combineIdxStart + dataDic.combineNum;

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

        ItemInfo findItemInfo = dataDic.findDic[idx];

        itemListPanel.transform.SetParent(tf);
        itemBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(findItemInfo.imagePath);

        if (DataController.GetInstance().itemOpenList.Contains(idx))
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

        infoWindow.itemImg.sprite = Resources.Load<Sprite>(itemInfo.imagePath);
        infoWindow.itemName.text = itemInfo.mtName;
        infoWindow.itemSort.text = itemInfo.group;
        infoWindow.itemGrade.text = itemInfo.grade;
        infoWindow.itemCost.text = "판매 가격 : " + itemInfo.sellPrice.ToString();
        infoWindow.itemText.text = itemInfo.description;
    }
}