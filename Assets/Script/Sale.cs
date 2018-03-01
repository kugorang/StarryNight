using UnityEngine;

public class Sale : MonoBehaviour
{
    private DataController dataController;
    private DialogueManager dialogueManager;

    private void Awake()
    {
        dataController = DataController.Instance;
        
    }

    private void Start()
    {
        dialogueManager = DialogueManager.Instance;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Material")
        {
            if(!dataController.IsTutorialEnd && dataController.NowIndex == 300423)
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