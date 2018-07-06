using UnityEngine;
using UnityEngine.EventSystems;

namespace Script
{
    public class Sale : MonoBehaviour
    {
        private DataController _dataController;

        private void Awake()
        {
            _dataController = DataController.Instance;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Material")) 
                return;
            
            AudioManager.GetInstance().SaleSound();
            var item = col.GetComponent<Item>();
            var itemInfo = item.Info;

            _dataController.Gold += (ulong) itemInfo.SellPrice;
            _dataController.DeleteItem(itemInfo.Index, item.Id);

            Destroy(col.gameObject);

            foreach (var target in _dataController.Observers) //관찰자들에게 이벤트 메세지 송출
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnSell(itemInfo));

            CameraController.FocusOnItem = false;
        }
    }
}