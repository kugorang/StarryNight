using UnityEngine;

public class Sale : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Material")
        {
            AudioManager.GetInstance().SaleSound();
            ItemInfo itemInfo = col.GetComponent<ItemInfo>();

            DataController.GetInstance().Gold+=(ulong)itemInfo.sellPrice;
            DataController.GetInstance().SubItemCount();
            DataController.GetInstance().DeleteItem(itemInfo.index);

            Destroy(col.gameObject);

            //CameraController.focusOnItem = false;
        }
    }
}