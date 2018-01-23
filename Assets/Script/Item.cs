using UnityEngine;

public class ItemInfo
{
    // 재료 기준 표 index
    public int Index { get; set; }

    // 재료 이름
    public string MtName { get; set; }

    // 재료 분류
    public string Group { get; set; }

    // 재료 등급
    public string Grade { get; set; }

    // 재료 판매 가격
    public int SellPrice { get; set; }

    // 재료 설명
    public string Description { get; set; }

    // 재료 이미지 경로
    public string ImagePath { get; set; }

    // 없어진지 확인하는 플래그
    public bool CheckDestroy { get; set; }

    public ItemInfo(int _index, string _name, string _group, string _grade, int _sellPrice, string _description, string _imagePath)
    {
        Index = _index;
        MtName = _name;
        Group = _group;
        Grade = _grade;
        SellPrice = _sellPrice;
        Description = _description;
        ImagePath = _imagePath;
        CheckDestroy = false;
    }
}

public class Item : MonoBehaviour
{
    public ItemInfo ItemInfo { get; set; }

    // 아이템 위치
    public Vector2 Pos { get; set; }

    // Use this for initialization
    private void Start()
    {
        // 아이템 생성 시 인벤토리 위치로 랜덤하게 이동
        iTween.MoveTo(gameObject, iTween.Hash("x", Random.Range(175, 953), "y", Random.Range(616, -227), "time", 1.0f, "easeType", iTween.EaseType.easeInExpo));
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 transfromPos = gameObject.transform.position;
        Pos = new Vector2(transfromPos.x, transfromPos.y);
    }

    public void SetItemInfo(int productID, ItemInfo findItemInfo)
    {
        ItemInfo = new ItemInfo(productID, findItemInfo.MtName, findItemInfo.Group, findItemInfo.Grade, findItemInfo.SellPrice, findItemInfo.Description, findItemInfo.ImagePath);
    }
}
