using UnityEngine;
using UnityEngine.UI;

public class BookListManager : MonoBehaviour
{
    int setIdxStart, setIdxMax;

    DataDictionary dataDic;

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
        dataDic = DataController.Instance.GetComponent<DataDictionary>();
        setContentPanel = GameObject.Find("SetContentPanel").transform;
    }

    private void Start()
    {
        int structNum = dataDic.SetComineList.Count;

        for (int index = 0; index < structNum; index++)
        {
            SetItemInfo setItemInfo = dataDic.SetComineList[index];

            AddItemButton(setItemInfo.index1, setContentPanel);
            AddItemButton(setItemInfo.index2, setContentPanel);
            AddItemButton(setItemInfo.index3, setContentPanel);
            AddItemButton(setItemInfo.index4, setContentPanel);
            AddItemButton(setItemInfo.result, setContentPanel);
        }
    }

    void AddItemButton(int idx, Transform tf)
    {
        GameObject itemListPanel = Instantiate(panelPrefab);
        Button itemBtn = itemListPanel.GetComponentInChildren<Button>();
        Image itemLock = itemListPanel.transform.Find("ItemLock").GetComponent<Image>();

        ItemInfo findItemInfo = dataDic.FindItemDic[idx];

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
        infoWindow.ItemName.text = itemInfo.Name;
        infoWindow.ItemSort.text = itemInfo.Group;
        infoWindow.ItemGrade.text = itemInfo.Grade;
        infoWindow.ItemCost.text = "판매 가격 : " + itemInfo.SellPrice.ToString();
        infoWindow.ItemText.text = itemInfo.Description;
    }
}