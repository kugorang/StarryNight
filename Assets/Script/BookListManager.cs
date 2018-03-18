using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
            //새 아이템이면 느낌표 표시
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
        //획득했으므로 더티 플래그와 느낌표 아이콘 갱신
        dataController.newBookList.Remove(idx);
        dataController.SaveGameData(dataController.newBookList, dataController.NewBookListPath);
        lockImg.gameObject.SetActive(false);
        //아이템 정보를 얻어 획득 보상 처리(최초 1회)
        ItemInfo item = dataDic.FindItemDic[idx];
        PopUpWindow.Alert(item.Name + " 획득 보상: "+item.SellPrice+" 골드", this);
        dataController.Gold += (ulong)item.SellPrice;
        
        foreach (GameObject target in dataController.Observers)//관찰자들에게 이벤트 메세지 송출
        {
            ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick<BookListManager>(this));
        }

    }
}