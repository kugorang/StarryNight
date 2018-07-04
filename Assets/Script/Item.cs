using UnityEngine;

namespace Script
{
    public class Item : MonoBehaviour
    {
        // Item 고유 번호
        public int Id { get; set; }

        // 아이템 위치
        public SerializableVector3 Pos { get; private set; }
        
        // 아이템 정보
        public ItemInfo Info { get; private set; }

        public void SetItemInfo(int productId, ItemInfo findItemInfo)
        {
            Info = new ItemInfo(productId, findItemInfo.Name, findItemInfo.Group, findItemInfo.Grade,
                findItemInfo.SellPrice, findItemInfo.Description, findItemInfo.ImagePath);
        }

        // TODO: 현재 절대 좌표로 되어 있는 코드를 상대 좌표로 변경할 것
        public void StartAnimation()
        {
            // 아이템 생성 시 인벤토리 위치로 랜덤하게 이동
            float randX = Random.Range(175, 953), randY = Random.Range(616, -227);

            iTween.MoveTo(gameObject,
                iTween.Hash("x", randX, "y", randY, "time", 1.0f, "easeType", iTween.EaseType.easeInExpo));

            Pos = new Vector3(randX, randY, -3);
        }
    }
}