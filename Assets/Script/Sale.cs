using UnityEngine;

public class Sale : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Material")
        {
            AudioManager.GetInstance().SaleSound();
            ItemInfo itemInfo = col.GetComponent<Item>().ItemInfo;

            DataController.Instance.Gold+=(ulong)itemInfo.SellPrice;
            DataController.Instance.SubItemCount();
            DataController.Instance.DeleteItem(itemInfo.Index);

            Destroy(col.gameObject);

            //CameraController.focusOnItem = false;
        }
    }
}