namespace Script
{
    public class ItemInfo
    {
        public ItemInfo(int index, string name, string @group, string grade, 
            int sellPrice, string description, string imagePath)
        {
            Index = index;
            Name = name;
            Group = @group;
            Grade = grade;
            SellPrice = sellPrice;
            Description = description;
            ImagePath = imagePath;
            CheckDestroy = false;
        }

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
    }
}