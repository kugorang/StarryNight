using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemTimer : MonoBehaviour
{
    public GameObject prefab;
    public Text timeDisplayer;  // 남은 시간 표시
    public Image img;
    public Button btn;
    public int index;
    float cooltime = 300.0f;    // 쿨타임 -> 타이머 쿨타임 업그레이드 추가 시 datacontroller에서 가져오는 걸로 수정 필요. itemtimer 2,3도 마찬가지
    public bool disableOnStart = false;

    private int sec, sec_1, sec_10, min;
    private DataDictionary dataDic;

    public Button combineButton;

    private DataController LeftTimer;
    private Vector3[] vectors = new Vector3[3] { new Vector3(-670, 772, -3), new Vector3(-550, 700, -3), new Vector3(-480, 772, -3) };

    private void Awake()
    {
        dataDic = GameObject.FindWithTag("DataController").GetComponent<DataDictionary>();
        LeftTimer = DataController.Instance;
    }

    void Start()
    {
        if (img == null)
            img = gameObject.GetComponent<Image>();

        if (btn == null)
            btn = gameObject.GetComponent<Button>();
    }

    // 시간당 게이지 채우기, 남은 시간 표시
    void Update()
    {
        if (LeftTimer[index] > 0)
        {
            btn.enabled = false;
            sec = (int)LeftTimer[index] % 60;
            sec_10 = sec / 10;
            sec_1 = sec % 10;
            min = (int)LeftTimer[index] / 60;
            timeDisplayer.text = min + ":" + sec_10 + sec_1;

           
            float ratio = 1.0f - (LeftTimer[index] / cooltime);

            if (img)
                img.fillAmount = ratio;
        }
        else
        {
            timeDisplayer.text = "0:00";
            img.fillAmount = 1.0f;

            LeftTimer[index] = 0;

            if (btn)
            {
                btn.enabled = true;
            }
        }
    }

    // 쿨타임 시간 지났는지 확인
    public bool CheckCooltime()
    {
        if (LeftTimer[index] > 0)
            return false;
        else
            return true;
    }

    public void ResetCooltime()
    {
        if (btn) // 버튼 활성화 시
        {
            /*if (DataController.Instance.ItemCount >= DataController.Instance.ItemLimit) // 아이템 갯수 차지 안 함.
            {
                Debug.Log("아이템 상자가 꽉 찼어요");
                return;
            }*/

            // 세트 아이템 랜덤 생성
            int id = Random.Range(4001, 4059);

            while (id % 5 == 0)
            {
                id = Random.Range(4001, 4059);
            }

            DataController.Instance.InsertNewItem(id,1); //도감에 등록만 되면 됨
            AudioManager.GetInstance().ItemSound();
            PopUpAlert.Alert("[서적] "+DataDictionary.Instance.FindItemDic[id].Name+" 획득",this);

            LeftTimer[index] = cooltime;
            btn.enabled = false;

            //DataController.Instance.AddItemCount(); 서적은 아이템 인벤토리에 해당 안 함
        }
    }

   /* private void CreateSetItem(int productID)
    {
        GameObject setItem = Instantiate(prefab, vectors[index], Quaternion.identity);


        CreateItem.Instance.GenerateItem(productID, true);
        setItem.GetComponent<Item>().SetItemInfo(productID, dataDic.FindItemDic[productID]);

        // 아래 주석 코드는 조합 버튼이 사라졌으므로 현재는 필요 없음.
        // 또 다시 쓸 수 있으므로 주석으로 비활성화함.
        //SetItemInfo setItemInfo = DataDictionary.GetInstance().CheckSetItemCombine(productID);

        //if (setItemInfo.result != 0)
        //{
        //    combineButton.gameObject.SetActive(true);
        //    combineButton.onClick.AddListener(() => OnClick(setItemInfo));
        //}

        setItem.GetComponent<BoxCollider2D>().isTrigger = false;
        setItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(dataDic.FindItemDic[productID].ImagePath);
    }
    */
    //void OnClick(SetItemInfo setItemInfo)
    //{
    //    DataController dataController = DataController.GetInstance();

    //    dataController.DeleteItem(setItemInfo.index1);
    //    dataController.DeleteItem(setItemInfo.index2);
    //    dataController.DeleteItem(setItemInfo.index3);
    //    dataController.DeleteItem(setItemInfo.index4);

    //    dataController.InsertNewItem(setItemInfo.result, 1);

    //    SceneManager.LoadScene("Main");
    //}
}