using UnityEngine;
using UnityEngine.UI;

public class BookListManager : MonoBehaviour
{
    int setIdxStart, setIdxMax;

    ItemDictionary itemDic;

    public GameObject panelPrefab, itemInfoPanel;

    Transform setContentPanel;

    private static BookListManager instance;

    public static BookListManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<BookListManager>();

            if (instance == null)
            {
                GameObject container = new GameObject("BookListManager");
                instance = container.AddComponent<BookListManager>();
            }
        }

        return instance;
    }

    private void Awake()
    {
        itemDic = DataController.GetInstance().GetComponent<ItemDictionary>();

        setIdxStart = 4000;
        setIdxMax = setIdxStart + itemDic.setNum;
        setContentPanel = GameObject.Find("SetContentPanel").transform;
    }

    private void Start()
    {
        for (int idx = setIdxStart + 1; idx <= setIdxMax; idx++)
        {
            AddItemButton(idx, setContentPanel);
        }
    }

    void AddItemButton(int idx, Transform tf)
    {
        GameObject itemListPanel = Instantiate(panelPrefab);
        Button itemBtn = itemListPanel.GetComponentInChildren<Button>();
        Image itemLock = itemListPanel.transform.Find("ItemLock").GetComponent<Image>();

        ItemInfo findItemInfo = itemDic.findDic[idx];
                
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