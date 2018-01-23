using UnityEngine;

public class Item : MonoBehaviour
{
    // Item 고유 번호
    public int Id { get; set; }

    // 아이템 위치
    public SerializableVector3 Pos { get; set; }

    public ItemInfo Info { get; private set; }

    public void SetItemInfo(int productID, ItemInfo findItemInfo)
    {
        Info = new ItemInfo(productID, findItemInfo.Name, findItemInfo.Group, findItemInfo.Grade, findItemInfo.SellPrice, findItemInfo.Description, findItemInfo.ImagePath);
    }

    public void StartAnimation()
    {
        // 아이템 생성 시 인벤토리 위치로 랜덤하게 이동
        float randX = Random.Range(175, 953), randY = Random.Range(616, -227);

        iTween.MoveTo(gameObject, iTween.Hash("x", randX, "y", randY, "time", 1.0f, "easeType", iTween.EaseType.easeInExpo));

        Pos = new Vector3(randX, randY, -3);
    }
}
