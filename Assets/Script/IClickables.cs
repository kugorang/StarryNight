using UnityEngine.EventSystems;

public interface IClickables : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void OnOtherClick();
}