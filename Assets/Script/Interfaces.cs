using UnityEngine;
using UnityEngine.EventSystems;

public enum ValueType
{
    Gold
}

public interface IClickables : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void OnOtherClick();
}

public interface IEventListener : IEventSystemHandler
{ 
    // functions that can be called via the messaging system
    //void OnClick(GameObject obj);
    void OnObjClick<T>(T obj, params int[] options);
    void OnSlide(bool isLeft, int sceneIndex);
    void OnObtain(ItemInfo item);
    void OnSell(ItemInfo item);
    void OnCombine(ItemInfo itemA, ItemInfo itemB, ItemInfo result);
    /// <summary>
    /// 값이 변화한 후 그것을 처리합니다.
    /// </summary>
    /// <param name="vt">변화한 값의 종류</param>
    /// <param name="args">0:변화 후 현재값 1:변화량</param>
    void OnChangeValue(ValueType vt,params float[] args);
    /// <summary>
    /// 값이 변화한 후 그것을 처리합니다.
    /// </summary>
    /// <param name="vt">변화한 값의 종류</param>
    /// <param name="args">0:변화 후 현재값 1:변화량(현재값-이후값)</param>
    void OnChangeValue(ValueType vt,params int[] args);
}

