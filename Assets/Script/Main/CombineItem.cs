#region

using System.Collections;
using System.Collections.Generic;
using Script.Common;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Script.Main
{
    public class CombineItem : MonoBehaviour
    {
        private DataController _dataController;
        private Dictionary<int, Dictionary<int, SerializableVector3>> _haveDic;
        private Item _item;
        private Vector3 _start;

        private void Awake()
        {
            _dataController = DataController.Instance;
            GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
            _haveDic = _dataController.HaveDic;
            _item = GetComponent<Item>();
        }

        private void OnMouseDown()
        {
            CameraController.FocusOnItem = true;
            _start = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 7);

            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in _dataController.Observers)
                // 무슨 아이템인지 알 수 있게 RaycastTest가 아니라 ItemInfo보냄 
                ExecuteEvents.Execute<IEventListener>(target, null,
                    (x, y) => x.OnObjClick(_item.Info, _item.Info.Index));
        }

        private void OnMouseDrag()
        {
            if (Camera.main != null)
                transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 7));
        }

        private void OnMouseUp()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            StartCoroutine(CheckCollision());
        }

        private IEnumerator CheckCollision()
        {
            // 0.1초 대기한다.
            yield return new WaitForSeconds(0.1f);

            // 이 사이 충돌이 감지 된다면 OnTriggerEnter2D로 간다.

            // 충돌 감지가 안 되었다면 아래 로직을 실행한다.
            GetComponent<BoxCollider2D>().isTrigger = false;

            // 아이템이 인벤토리 밖으로 벗어날 경우 아이템 드래그 전 위치로 다시 이동
            if (Input.mousePosition.x > 7 * (float) Screen.width / 8
                || Input.mousePosition.x < (float) Screen.width / 8
                || Input.mousePosition.y > 7 * (float) Screen.height / 8
                || Input.mousePosition.y < (float) Screen.height / 3)
                if (Camera.main != null)
                    transform.position = Camera.main.ScreenToWorldPoint(_start);

            _haveDic[_item.Info.Index][_item.Id] = transform.position;
            DataController.SaveGameData(_haveDic, _dataController.HaveDicPath);

            CameraController.FocusOnItem = false;
        }

        private void OnTriggerEnter2D(Collider2D collision) //TODO: 상위 아이템 생성확률 추가
        {
            if (!collision.CompareTag("Material") || !collision.isTrigger)
                return;

            var collItem = collision.GetComponent<Item>();
            var collItemInfo = collItem.Info;

            if (collItemInfo.CheckDestroy)
                return;

            // 조합표에 있는 조합식인지 검색한다.
            var dataDic = DataDictionary.Instance;
            var myItemInfo = GetComponent<Item>().Info;

            var key1 = myItemInfo.Index;
            var key2 = collItemInfo.Index;
            var resultList = dataDic.FindCombine(key1, key2);

            if (resultList == null)
            {
                PopUpWindow.Alert("조합할 수 없어요");
                return;
            }

            collItemInfo.CheckDestroy = true;

            // 조합표에 있다면 충돌 당한 물체를 결과 재료로 바꾸어준다.
            // 조합 결과 개수를 얻어온다.
            var resultNum = resultList.Count;
            var findItemInfo = dataDic.FindItem(resultList[Random.Range(0, resultNum)]);

            // 관찰자들에게 이벤트 메세지 송출, myItemInfo 바뀌기 전 정보 보냄.
            foreach (var target in _dataController.Observers)
                ExecuteEvents.Execute<IEventListener>(target, null,
                    (x, y) => x.OnCombine(myItemInfo, collItemInfo, findItemInfo));

            myItemInfo.Index = findItemInfo.Index;
            myItemInfo.Name = findItemInfo.Name;
            myItemInfo.Group = findItemInfo.Group;
            myItemInfo.Grade = findItemInfo.Grade;
            myItemInfo.SellPrice = findItemInfo.SellPrice;
            myItemInfo.Description = findItemInfo.Description;
            myItemInfo.ImagePath = findItemInfo.ImagePath;

            GetComponent<SpriteRenderer>().sprite =
                Resources.Load<Sprite>(dataDic.FindItemDic[myItemInfo.Index].ImagePath);

            // 충돌한 물체를 가지고 있는 재료 dictionary에서 삭제한다.
            _dataController.DeleteItem(key2, collItem.Id);
            _dataController.DeleteItem(key1, GetComponent<Item>().Id);

            _dataController.InsertNewItem(myItemInfo.Index, GetComponent<Item>().Id, transform.position);

            // 조합 후 충돌한 물체를 파괴한다.
            Destroy(collision.gameObject);
            AudioManager.Instance.MixSound();

            // 조합표에 없다면 그냥 무시한다.
            CameraController.FocusOnItem = false;
        }
    }
}