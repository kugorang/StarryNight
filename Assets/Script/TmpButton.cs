using UnityEngine;
using UnityEngine.EventSystems;

namespace Script
{
    public class TmpButton : MonoBehaviour
    {
        public void OnClick()
        {
            /*Debug.Log("TmpButton : " + DataController.Instance.NowIndex);*/
        
            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in DataController.Instance.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));
        }
    }
}