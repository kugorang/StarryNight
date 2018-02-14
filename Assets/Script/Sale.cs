using UnityEngine;

public class Sale : MonoBehaviour
{
    private DataController dataController;
    public DialogueManager dialogueManager;

    private void Awake()
    {
        dataController = DataController.Instance;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Material")
        {
            if(dataController.IsTutorialEnd == 0 && dataController.NowIndex == 300423)
            {
                dialogueManager.ContinueDialogue();
            }

            AudioManager.GetInstance().SaleSound();
            Item item = col.GetComponent<Item>();
            ItemInfo itemInfo = item.Info;

            dataController.Gold += (ulong)itemInfo.SellPrice;
            dataController.ItemCount -= 1;
            dataController.DeleteItem(itemInfo.Index, item.Id);

            Destroy(col.gameObject);

            //CameraController.focusOnItem = false;
        }
    }
}