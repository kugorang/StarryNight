using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateItem : MonoBehaviour
{
    private int energy = 0;  // 에너지 
    private int energyPerClick; // 클릭당 에너지 증가량
    private int energyMaxValue = 100; // 에너지 충전 최대량
    public GameObject item; // 아이템
    public Image img_earthback; // 게이지 이미지
    public Button btn;

    private ItemDictionary itemDic;

    public Button combineButton;

    private static CreateItem instance;

    public static CreateItem GetInstance()
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
    
    private void Awake()
    {
        energyPerClick = DataController.GetInstance().EnergyPerClick;
        itemDic = GameObject.FindWithTag("DataController").GetComponent<ItemDictionary>();

        //combineButton.gameObject.SetActive(false); 개편으로 필요성 상실
    }

    void Start()
    {
        if (img_earthback == null)
            img_earthback = GameObject.Find("EarthBack").GetComponent<Image>();

        if (btn == null)
            btn = gameObject.GetComponent<Button>();

        img_earthback.fillAmount = 0.0f; // 처음 버튼 게이지 0으로 -> 게이지 저장 가능 시 삭제해야함

        List<SetItemInfo> tmpSetItemInfo = new List<SetItemInfo>();

        if (DataController.GetInstance().haveDic != null)
        {

            foreach (KeyValuePair<int, int> entry in DataController.GetInstance().haveDic)
            {
                // do something with entry.Value or entry.Key
                for (int i = 0; i < entry.Value; i++)
                {
                    GenerateItem(entry.Key, false);

                    SetItemInfo setItemInfo = ItemDictionary.GetInstance().CheckSetItemCombine(entry.Key);

                    if (setItemInfo.result != 0 && !tmpSetItemInfo.Contains(setItemInfo))
                    {
                        tmpSetItemInfo.Add(setItemInfo);

                        combineButton.gameObject.SetActive(true);
                        combineButton.onClick.AddListener(() => OnClick(setItemInfo));
                    }
                }
            }
        }

        energy = DataController.GetInstance().Energy;

        img_earthback.fillAmount = (float)energy / energyMaxValue;
    }

    public void AddEnergy() // 클릭 수 증가
    {
        energy += energyPerClick;
        img_earthback.fillAmount = (float)energy / energyMaxValue;

        DataController.GetInstance().Energy=energy;
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

        energy = 0;
        DataController.GetInstance().Energy=energy;
        btn.enabled = true;

        yield return null;
    }

    // 게이지 클릭
    public void OnClick()
    {
        AudioManager.GetInstance().ClickSound();
        AddEnergy();
        NewObject();
    }

    // 아이템 생성
    private void NewObject()
    {
        if (energy >= energyMaxValue)
        {
            if (DataController.GetInstance().ItemCount >= DataController.GetInstance().ItemLimit) // 아이템 갯수 제한
            {
                Debug.Log("아이템 상자가 꽉 찼어요~");

                return;
            }

            if (SwitchSunMoon.GetInstance().GetState() == 1) // sun일 때 나뭇가지 등 생성해야함
            {
                if (Random.Range(0, 100) >= 95)
                {
                    GenerateItem(Random.Range(2007, 2013), true);
                }
                else
                {
                    GenerateItem(Random.Range(2001, 2007), true);
                }
            }
            else
            {
                if (Random.Range(0, 100) >= 95)
                {
                    GenerateItem(Random.Range(1004, 1007), true);
                }
                else
                {
                    GenerateItem(Random.Range(1001, 1004), true);
                }
            }

            DataController.GetInstance().AddItemCount();
            ResetEnergy();

            AudioManager.GetInstance().ItemSound();

            //img.fillAmount = 0.0f;
        }
    }

    public void GenerateItem(int productID, bool isNew)
    {
        //Instantiate(stick, new Vector3(-213, -396, 0), Quaternion.identity).transform.SetParent(GameObject.Find("Canvas").transform, false); // canvas 자식으로 상속해서 prifab생성
        GameObject newItem = Instantiate(item, new Vector3(-758, -284, -3), Quaternion.identity);

        // 현재 보유하고 있는 재료를 관리하는 Dictionary에 방금 생성한 item을 넣어준다.
        if (isNew)
        {
            DataController.GetInstance().InsertItem(productID, 1);
        }

        ItemInfo itemInfo = newItem.GetComponent<ItemInfo>();
        ItemInfo findItemInfo = itemDic.findDic[productID];

        itemInfo.index = productID;
        itemInfo.mtName = findItemInfo.mtName;
        itemInfo.group = findItemInfo.group;
        itemInfo.grade = findItemInfo.grade;
        itemInfo.sellPrice = findItemInfo.sellPrice;
        itemInfo.description = findItemInfo.description;
        itemInfo.imagePath = findItemInfo.imagePath;

        newItem.GetComponent<BoxCollider2D>().isTrigger = false;
        newItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(itemDic.findDic[productID].imagePath);
    }

    void OnClick(SetItemInfo setItemInfo)
    {
        DataController dataController = DataController.GetInstance();

        dataController.DeleteItem(setItemInfo.index1);
        dataController.DeleteItem(setItemInfo.index2);
        dataController.DeleteItem(setItemInfo.index3);
        dataController.DeleteItem(setItemInfo.index4);

        dataController.SubItemCount();
        dataController.SubItemCount();
        dataController.SubItemCount();
        
        dataController.InsertItem(setItemInfo.result, 1);

        SceneManager.LoadScene("Main");
    }
}