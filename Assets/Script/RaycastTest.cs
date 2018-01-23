using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    Vector3 start;

    private void OnMouseDown()
    {
        CameraController.FocusOnItem = true;
        //Debug.Log("FocusOnItem : " + CameraController.FocusOnItem);
        start = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 7);
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 7));
    }

    private void OnMouseUp()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        StartCoroutine(CheckCollision());
    }

    IEnumerator CheckCollision()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<BoxCollider2D>().isTrigger = false;
        CameraController.FocusOnItem = false;
        Debug.Log("FocusOnItem : " + CameraController.FocusOnItem);

        // 아이템이 인벤토리 밖으로 벗어날 경우 아이템 드래그 전 위치로 다시 이동
        if (Input.mousePosition.x > 7 * Screen.width / 8 || Input.mousePosition.x < Screen.width / 8 || Input.mousePosition.y > 7 * Screen.height / 8 || Input.mousePosition.y < Screen.height / 3)
            transform.position = Camera.main.ScreenToWorldPoint(start);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Material" && collision.isTrigger)
        {
            ItemInfo collItemInfo = collision.GetComponent<Item>().ItemInfo;

            if (collItemInfo.CheckDestroy)
            {
                return;
            }

            // 조합표에 있는 조합식인지 검색한다.
            DataDictionary dataDic = GameObject.FindWithTag("DataController").GetComponent<DataDictionary>();

            ItemInfo myItemInfo = GetComponent<Item>().ItemInfo;

            int key1 = myItemInfo.Index;
            int key2 = collItemInfo.Index;
            List<int> resultList = dataDic.FindCombine(key1, key2);

            if (resultList != null)
            {
                // 조합표에 있다면 충돌 당한 물체를 결과 재료로 바꾸어준다.
                //Debug.Log("result != 0");

                // 조합 결과 개수를 얻어온다.
                int resultNum = resultList.Count;

                ItemInfo findItemInfo = dataDic.FindItem(resultList[Random.Range(0, resultNum)]);

                myItemInfo.Index = findItemInfo.Index;
                myItemInfo.MtName = findItemInfo.MtName;
                myItemInfo.Group = findItemInfo.Group;
                myItemInfo.Grade = findItemInfo.Grade;
                myItemInfo.SellPrice = findItemInfo.SellPrice;
                myItemInfo.Description = findItemInfo.Description;
                myItemInfo.ImagePath = findItemInfo.ImagePath;

                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(dataDic.FindDic[myItemInfo.Index].ImagePath);

                // 충돌한 물체를 가지고 있는 재료 dictionary에서 삭제한다.
                DataController.Instance.DeleteItem(key2);
                DataController.Instance.DeleteItem(key1);

                DataController.Instance.InsertItem(myItemInfo.Index, 1);

                // 조합 후 충돌한 물체를 파괴한다.
                collItemInfo.CheckDestroy = true;

                Destroy(collision.gameObject);

                DataController.Instance.SubItemCount();
            }
            //else
            //{
            //    Debug.Log("result == 0");
            //}

            // 조합표에 없다면 그냥 무시한다.
        }

        StartCoroutine(CheckCollision());
    }
}