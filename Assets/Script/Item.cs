using UnityEngine;

public class ItemInfo
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

    public ItemInfo(int _index, string _name, string _group, string _grade, int _sellPrice, string _description, string _imagePath)
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

public class Item : MonoBehaviour
{
    public ItemInfo itemInfo { get; set; }

    // 아이템 위치
    public Vector2 pos { get; set; }

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
        pos = new Vector2(transfromPos.x, transfromPos.y);
    }

    public void SetItemInfo(int productID, ItemInfo findItemInfo)
    {
        itemInfo = new ItemInfo(productID, findItemInfo.mtName, findItemInfo.group, findItemInfo.grade, findItemInfo.sellPrice, findItemInfo.description, findItemInfo.imagePath);
    }
}
