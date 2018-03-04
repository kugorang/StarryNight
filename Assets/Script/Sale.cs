using UnityEngine;
using UnityEngine.EventSystems;

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
            

            AudioManager.GetInstance().SaleSound();
            Item item = col.GetComponent<Item>();
            ItemInfo itemInfo = item.Info;

            dataController.Gold += (ulong)itemInfo.SellPrice;
            dataController.ItemCount -= 1;
            dataController.DeleteItem(itemInfo.Index, item.Id);

            Destroy(col.gameObject);

            foreach (GameObject target in dataController.Observers)//관찰자들에게 이벤트 메세지 송출
            {
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnSell(itemInfo));
            }

            //CameraController.focusOnItem = false;
        }
    }
}