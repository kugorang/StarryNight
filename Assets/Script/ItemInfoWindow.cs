using UnityEngine;
using UnityEngine.UI;

public class ItemInfoWindow : MonoBehaviour
{
    public GameObject itemInfoPanel;

    public Image itemImg { get; set; }
    public Text itemName { get; set; }
    public Text itemSort { get; set; }
    public Text itemGrade { get; set; }
    public Text itemCost { get; set; }
    public Text itemText { get; set; }

    private void Awake()
    {
        itemImg = GameObject.Find("ItemImage").GetComponent<Image>();
        itemName = GameObject.Find("ItemName").GetComponent<Text>();
        itemSort = GameObject.Find("ItemSort").GetComponent<Text>();
        itemGrade = GameObject.Find("ItemGrade").GetComponent<Text>();
        itemCost = GameObject.Find("ItemCost").GetComponent<Text>();
        itemText = GameObject.Find("ItemText").GetComponent<Text>();

        itemInfoPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
        itemInfoPanel.SetActive(false);
    }
}