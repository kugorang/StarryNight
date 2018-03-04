using UnityEngine;
using UnityEngine.EventSystems;

public class TmpButton : MonoBehaviour
{
    public void OnClick()
    {
        foreach (GameObject target in DataController.Instance.Observers)//관찰자들에게 이벤트 메세지 송출
        {
            ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick<TmpButton>(this));
        }

    }
}