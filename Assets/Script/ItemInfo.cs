public class ItemInfo
{
    // 재료 기준 표 Index
    public int Index { get; set; }

    // 재료 이름
    public string Name { get; set; }

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

    // 없어진지 확인하는 플래 그
    public bool CheckDestroy { get; set; }

    public ItemInfo(int _index, string _name, string _group, string _grade, int _sellPrice, string _description, string _imagePath)
    {
        Index = _index;
        Name = _name;
        Group = _group;
        Grade = _grade;
        SellPrice = _sellPrice;
        Description = _description;
        ImagePath = _imagePath;
        CheckDestroy = false;
    }
}