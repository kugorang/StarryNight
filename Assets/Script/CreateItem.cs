using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateItem : MonoBehaviour
{
    private int energyPerClick;         // 클릭당 에너지 증가량
    private int energyMaxValue;         // 에너지 충전 최대량
    public GameObject item;             // 아이템
    public Image img_earthback;         // 게이지 이미지
    public Button btn;
    //public GameObject alarmWindow;      // 알림창

    private DataDictionary dataDic;

    public Button combineButton;

    private DataController dataController;
    private DialogueManager dialogueManager;

    // Item ID 공유 변수
    public static int IdCount;

    private static CreateItem instance;

    public static CreateItem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CreateItem>();

                if (instance == null)
                {
                    GameObject container = new GameObject("CreateItem");
                    instance = container.AddComponent<CreateItem>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        dataController = DataController.Instance;
        
        dataDic = DataDictionary.Instance;
        //energyPerClick = DataController.Instance.EnergyPerClick;
        energyPerClick = 100;   // 개발 시 시간 절약을 위해 100으로 설정.
        energyMaxValue = 100;

        IdCount = PlayerPrefs.GetInt("IdCount", 0);
    }


    void Start()
    {
        dialogueManager = DialogueManager.Instance;
        if (img_earthback == null)
        {
            img_earthback = GameObject.Find("EarthBack").GetComponent<Image>();
        }

        if (btn == null)
        {
            btn = gameObject.GetComponent<Button>();
        }

        //List<SetItemInfo> tmpSetItemInfo = new List<SetItemInfo>();

        if (dataController.HaveDic != null)
        {
            foreach (KeyValuePair<int, Dictionary<int, SerializableVector3>> entry in dataController.HaveDic)
            {
                if (entry.Key > 4000)//서적이면 만들지 않는다.
                {
                    continue;
                }
                // do something with entry.Value or entry.Key                
                foreach (KeyValuePair<int, SerializableVector3> secondEntry in entry.Value)
                {
                    GenerateItem(entry.Key, false, secondEntry.Key, secondEntry.Value);
                }

                //int loopNum = entry.Value.Count;

                //for (int index = 0; index < loopNum; index++)
                //{
                //    GenerateItem(entry.Key, false, entry.Value[index]);

                //    //SetItemInfo setItemInfo = DataDictionary.GetInstance().CheckSetItemCombine(entry.Key);

                //    //if (setItemInfo.result != 0 && !tmpSetItemInfo.Contains(setItemInfo))
                //    //{
                //    //    tmpSetItemInfo.Add(setItemInfo);

                //    //    combineButton.gameObject.SetActive(true);
                //    //    combineButton.onClick.AddListener(() => OnClick(setItemInfo));
                //    //}
                //}
            }
        }

        img_earthback.fillAmount = (float)dataController.Energy / energyMaxValue;
    }

    public void AddEnergy() // 클릭 수 증가
    {
        int energy = dataController.Energy + energyPerClick;
        img_earthback.fillAmount = energy / energyMaxValue;
        dataController.Energy = energy;
    }

    public void ResetEnergy() // 클릭 수 초기화
    {
        btn.enabled = false;
        StartCoroutine(DecreaseEnergy());
    }

    // 에너지 감소
    IEnumerator DecreaseEnergy()
    {
        while (img_earthback.fillAmount != 0)
        {
            yield return new WaitForSeconds(0.05f);

            img_earthback.fillAmount -= 0.1f;
        }

        dataController.Energy = 0;
        btn.enabled = true;

        yield return null;
    }

    // 게이지 클릭
    public void OnClick()
    {
        foreach (GameObject target in dataController.Observers)//관찰자들에게 이벤트 메세지 송출
        {
            ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick<CreateItem>(this));
        }

        AudioManager.GetInstance().ClickSound();
        AddEnergy();
        NewObject();
    }

    // 아이템 생성
    private void NewObject()
    {
        if (dataController.Energy >= energyMaxValue)
        {
            if (dataController.ItemCount >= dataController.ItemLimit) // 아이템 갯수 제한
            {
                //StartCoroutine("ShowAlertWindow");
                PopUpWindow.Alert("아이템 상자가 꽉 찼어요.",this);
                return;
            }

            if (SwitchSunMoon.Instance.State) // sun일 때 나뭇가지 등 생성해야함

            {
                if (UnityEngine.Random.Range(0, 100) >= dataController.AtlasItemProb)
                {
                    GenerateItem(UnityEngine.Random.Range(2007, 2013), true);
                }
                else
                {
                    GenerateItem(UnityEngine.Random.Range(2001, 2007), true);
                }
            }
            else
            {
                if (UnityEngine.Random.Range(0, 100) >= dataController.AtlasItemProb)
                {
                    GenerateItem(UnityEngine.Random.Range(1004, 1007), true);
                }
                else
                {
                    GenerateItem(UnityEngine.Random.Range(1001, 1004), true);
                }
            }

            dataController.ItemCount += 1;
            ResetEnergy();
            AudioManager.GetInstance().ItemSound();

           


            
        }
    }

    private void StartCoroutine(Func<IEnumerator> showAlertWindow)
    {
        throw new NotImplementedException();
    }

    // C#에서는 디폴트 파라미터를 허용하지 않기 때문에 이렇게 함수 오버로딩을 통해 구현하였습니다.
    public void GenerateItem(int productID, bool isNew)
    {
        GenerateItem(productID, isNew, -1, new Vector3(-758, -284, -3));
    }

    public void GenerateItem(int productID, bool isNew, int itemID, Vector3 itemPos)
    {
        if (itemPos.x<0)
        {
            Debug.LogWarning("Generated in wrong place.");
        }
        GameObject newItem = Instantiate(item, itemPos, Quaternion.identity);

        ItemInfo findItemInfo = dataDic.FindItemDic[productID];
        Item itemInstance = newItem.GetComponent<Item>();
        itemInstance.SetItemInfo(productID, findItemInfo);

        newItem.GetComponent<BoxCollider2D>().isTrigger = false;
        newItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(dataDic.FindItemDic[productID].ImagePath);

        // 새로 만들어진 아이템이라면 애니메이션을 실행한다.
        if (isNew)
        {
            itemInstance.Id = IdCount++;
            PlayerPrefs.SetInt("IdCount", IdCount);

            itemInstance.StartAnimation();
            dataController.InsertNewItem(productID, itemInstance.Id, itemInstance.Pos);
        }
        else
        {
            itemInstance.Id = itemID;
            dataController.SaveGameData(dataController.HaveDic, dataController.HaveDicPath);
        }

        
    }

    /*IEnumerator ShowAlertWindow()
    {
        alarmWindow.SetActive(true);
        alarmWindow.GetComponentInChildren<Text>().text = "아이템 상자가 꽉 찼습니다.";
        yield return new WaitForSeconds(3.0f);
        alarmWindow.SetActive(false);
    }*/

    //void OnClick(SetItemInfo setItemInfo)
    //{
    //    dataController.DeleteItem(setItemInfo.index1);
    //    dataController.DeleteItem(setItemInfo.index2);
    //    dataController.DeleteItem(setItemInfo.index3);
    //    dataController.DeleteItem(setItemInfo.index4);

    //    dataController.SubItemCount();
    //    dataController.SubItemCount();
    //    dataController.SubItemCount();

    //    dataController.InsertNewItem(setItemInfo.result, 1);

    //    SceneManager.LoadScene("Main");
    //}
}