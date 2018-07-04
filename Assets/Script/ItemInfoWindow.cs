using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class ItemInfoWindow : MonoBehaviour
    {
        public GameObject ItemInfoPanel;

        public Image ItemImg { get; private set; }
        public Text ItemName { get; private set; }
        public Text ItemSort { get; private set; }
        public Text ItemGrade { get; private set; }
        public Text ItemCost { get; private set; }
        public Text ItemText { get; private set; }

        private void Awake()
        {
            ItemImg = GameObject.Find("ItemImage").GetComponent<Image>();
            ItemName = GameObject.Find("ItemName").GetComponent<Text>();
            ItemSort = GameObject.Find("ItemSort").GetComponent<Text>();
            ItemGrade = GameObject.Find("ItemGrade").GetComponent<Text>();
            ItemCost = GameObject.Find("ItemCost").GetComponent<Text>();
            ItemText = GameObject.Find("ItemText").GetComponent<Text>();

            ItemInfoPanel.SetActive(false);
            gameObject.SetActive(false);
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);
            ItemInfoPanel.SetActive(false);
        }
    }
}