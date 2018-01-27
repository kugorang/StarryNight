using UnityEngine;

public class Sale : MonoBehaviour
{
    DataController dataController;

    private void Awake()
    {
        dataController = DataController.Instance;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Material")
        {
            AudioManager.GetInstance().SaleSound();
            Item item = col.GetComponent<Item>();
            ItemInfo itemInfo = item.Info;

            dataController.Gold += (ulong)itemInfo.SellPrice;
            dataController.SubItemCount();
            dataController.DeleteItem(itemInfo.Index, item.Id);


            Destroy(col.gameObject);

            //CameraController.focusOnItem = false;
        }
    }
}