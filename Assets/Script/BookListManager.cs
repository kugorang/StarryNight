using UnityEngine;
using UnityEngine.UI;

public class BookListManager : MonoBehaviour
{
    int setIdxStart, setIdxMax;
    DataDictionary dataDic;
    public GameObject panelPrefab, itemInfoPanel;
    public Sprite NewItemAlert;
    Transform setContentPanel;
    private DataController dataController;
    private DialogueManager dialogueManager;

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
        dataController = DataController.Instance;
        
        dialogueManager = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
        dataDic = DataController.Instance.GetComponent<DataDictionary>();
        setContentPanel = GameObject.Find("SetContentPanel").transform;
    }

    private void Start()
    {
        dialogueManager = DialogueManager.Instance;
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
            ColorBlock btnColors = itemBtn.colors;

            btnColors.normalColor = Color.white;
            btnColors.highlightedColor = Color.white;
            btnColors.pressedColor = Color.white;

            itemBtn.colors = btnColors;

            itemBtn.onClick.AddListener(() => ShowWindow(findItemInfo));
        }
        if (dataController.newBookList.Contains(idx))
        {
            //새 아이템 표시 추가할 것
            itemLock.sprite = NewItemAlert;
            itemLock.raycastTarget = false;
            itemBtn.onClick.AddListener(() => RemoveAlert(idx,itemLock));
        }
        else
        {
            itemLock.gameObject.SetActive(false);
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
        infoWindow.ItemCost.text = "획득 보상 : " + itemInfo.SellPrice.ToString()+ " 골드";
        infoWindow.ItemText.text = itemInfo.Description;
    }

    public void RemoveAlert(int idx, Image lockImg)
    {  
        dataController.newBookList.Remove(idx);
        dataController.SaveGameData(dataController.newBookList, dataController.NewBookListPath);
        lockImg.gameObject.SetActive(false);
        ItemInfo item = dataDic.FindItemDic[idx];
        PopUpWindow.Alert(item.Name + " 획득 보상: "+item.SellPrice+" 골드", this, true);
        dataController.Gold += (ulong)item.SellPrice;
        if (!dataController.IsTutorialEnd && (dataController.NowIndex == 300612 || dataController.NowIndex == 300623))
        {
            dialogueManager.ContinueDialogue();
        }
    }
}