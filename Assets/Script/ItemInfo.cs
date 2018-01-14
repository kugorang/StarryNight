using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    // 재료 기준 표 index
    public int index { get; set; }

    // 재료 이름
    public string mtName { get; set; }

    // 재료 분류
    public string group { get; set; }

    // 재료 등급
    public string grade { get; set; }

    // 재료 판매 가격
    public int sellPrice { get; set; }

    // 재료 설명
    public string description { get; set; }

    // 재료 이미지 경로
    public string imagePath { get; set; }

    // 없어진지 확인하는 플래그
    public bool checkDestroy { get; set; }

    public void Init(int _index, string _name, string _group, string _grade, int _sellPrice, string _description, string _imagePath)
    {
        index = _index;
        mtName = _name;
        group = _group;
        grade = _grade;
        sellPrice = _sellPrice;
        description = _description;
        imagePath = _imagePath;
        checkDestroy = false;
    }
}