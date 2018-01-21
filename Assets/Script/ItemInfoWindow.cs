using UnityEngine;
using UnityEngine.UI;

public class ItemInfoWindow : MonoBehaviour
{
    public GameObject itemInfoPanel;

    public Image ItemImg { get; set; }
    public Text ItemName { get; set; }
    public Text ItemSort { get; set; }
    public Text ItemGrade { get; set; }
    public Text ItemCost { get; set; }
    public Text ItemText { get; set; }

    private void Awake()
    {
        ItemImg = GameObject.Find("ItemImage").GetComponent<Image>();
        ItemName = GameObject.Find("ItemName").GetComponent<Text>();
        ItemSort = GameObject.Find("ItemSort").GetComponent<Text>();
        ItemGrade = GameObject.Find("ItemGrade").GetComponent<Text>();
        ItemCost = GameObject.Find("ItemCost").GetComponent<Text>();
        ItemText = GameObject.Find("ItemText").GetComponent<Text>();

        itemInfoPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
        itemInfoPanel.SetActive(false);
    }
}