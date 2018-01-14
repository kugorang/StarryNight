using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCost : MonoBehaviour {

    private static ItemCost instance;

    public static ItemCost GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<ItemCost>();

            if (instance == null)
            {
                GameObject container = new GameObject("ItemCost");

                instance = container.AddComponent<ItemCost>();
            }
        }
        return instance;
    }


    private int cost = 10;

    /// <summary>
    /// item 가격 가져오는 함수
    /// </summary>
    /// <returns></returns>
    public int GetCost()
    {
        return cost;
    }


    /// <summary>
    /// item 가격 설정 함수
    /// </summary>
    /// <param name="newCost">설정할 item 가격</param>
    public void SetCost(int newCost)
    {
        cost = newCost;
    }

}
