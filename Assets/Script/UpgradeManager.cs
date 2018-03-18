using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeManager : MonoBehaviour
{
    private DataDictionary dataDic;

    // 업그레이드 잠금 표시
    private Transform[] ParentPanels;
    private GameObject[] UnlockPanels=new GameObject[12];
    private Text[] UpgradeDisplayers=new Text[12];
    private Text[] UpgradeCostDisplayers = new Text[12];
    private Button[] UpgradeButtons = new Button[12];
    private UpgradeClass currentUpgradeLV = DataController.upgradeLV;

    private const int MAX_LEVEL = 20;
    private const int LAST_UPGRADE_MAX_LEVEL = 1;


    public int[] successRate;
 


    private DataController dataController;
    private DialogueManager dialogueManager;

    private void Awake()
    {
        dataDic = GameObject.FindWithTag("DataController").GetComponent<DataDictionary>();

        ParentPanels = new Transform[12]
        {
            GameObject.Find("Upgrade Panel (0)").transform,
            GameObject.Find("Upgrade Panel (1)").transform,
            GameObject.Find("Upgrade Panel (2)").transform,
            GameObject.Find("Upgrade Panel (3)").transform,
            GameObject.Find("Upgrade Panel (4)").transform,
            GameObject.Find("Upgrade Panel (5)").transform,
            GameObject.Find("Upgrade Panel (6)").transform,
            GameObject.Find("Upgrade Panel (7)").transform,
            GameObject.Find("Upgrade Panel (8)").transform,
            GameObject.Find("Upgrade Panel (9)").transform,
            GameObject.Find("Upgrade Panel (10)").transform,
            GameObject.Find("Upgrade Panel (11)").transform
        };

        for(int i=0; i<12; i++)
        {
            UnlockPanels[i] = ParentPanels[i].Find("Unlock Panel").gameObject;
            UpgradeDisplayers[i] = ParentPanels[i].Find("Upgrade Displayer").gameObject.GetComponent<Text>();
            UpgradeButtons[i] = ParentPanels[i].Find("Upgrade Button").gameObject.GetComponent<Button>();
            UpgradeCostDisplayers[i]= UpgradeButtons[i].transform.Find("Upgrade Cost Displayer").gameObject.GetComponent<Text>();
        }
        dataController = DataController.Instance;
        
        if (successRate.Length < 12)
        {
            successRate = new int[12];
        }
    }

    private void Start()
    {
        dialogueManager = DialogueManager.Instance;
    }

    void Update()
    {
        for(int i=0; i < 12; i++)
        {
            int upgradeIndex = 50001 + i;
            //업그레이드 text 및 버튼 설정
            if ( upgradeIndex> dataController.LatestUpgradeIndex)
            {
                UpgradeButtons[i].enabled = false;
                if ( IsMaxUpgraded(upgradeIndex))
                {
                    UnlockPanels[i].SetActive(false);
                }
                else
                {
                    UnlockPanels[i].SetActive(true);
                }
            }
            else
            {               
                UpgradeButtons[i].enabled = true;
                UnlockPanels[i].SetActive(false);
            }
            // value[current값] -> 꼭!
            if (IsMaxUpgraded(upgradeIndex))

            {
                UpgradeDisplayers[i].text = "MAX";
                UpgradeCostDisplayers[i].text = "MAX";
            }
            else
            {
                int nextUpgradeValue = dataDic.FindUpDic[upgradeIndex].value[currentUpgradeLV[i]];
                string str="";
                switch (i)
                {
                    case 0:
                        str = "인벤토리 +" + DataController.Instance.ItemLimit + " -> +" + (10+nextUpgradeValue);
                        break;
                    case 1:
                        str="클릭당 게이지 +" + DataController.Instance.EnergyPerClick + " -> +" + (2 + nextUpgradeValue);
                        break;
                    case 2:
                        str = "아이템 타이머1 시간 감소 -" + DataController.Instance + "초 -> -" + nextUpgradeValue + "초";
                        break;
                    case 3:
                        str = "아이템 타이머1에서 좋은 아이템 나올 확률 증가 +" + DataController.Instance + "% -> +" + nextUpgradeValue + "%";
                        break;
                    case 4:
                        str = "판매시 골드 추가 지급 +" + DataController.Instance + "골드 -> +" + nextUpgradeValue + "골드";
                        break;
                    case 5:
                        str = "아이템 타이머2 시간 감소 -" + DataController.Instance + "초 -> -" + nextUpgradeValue + "초";
                        break;
                    case 6:
                        str = "아이템 타이머2에서 좋은 아이템 나올 확률 증가 +" + DataController.Instance + "% -> +" + nextUpgradeValue + "%";
                        break;
                    case 7:
                        str = "지구본에서 한 단계 상위 아이템 나올 확률 증가" + DataController.Instance + "% -> +" + nextUpgradeValue + "%";
                        break;
                    case 8:
                        str = "아이템 조합 시 한 단계 상위 아이템 나올 확률 증가 +" + DataController.Instance + "% -> +" + nextUpgradeValue + "%";
                        break;
                    case 9:
                        str = "아이템 타이머3 시간 감소 -" + DataController.Instance + "초 -> -" + nextUpgradeValue + "초";
                        break;
                    case 10:
                        str = "아이템 타이머3에서 좋은 아이템 나올 확률 증가 +" + DataController.Instance + "% -> +" + nextUpgradeValue + "%";
                        break;
                    default://최종업그레이드
                        str = "골드 2배, 인벤토리 2배, 높은 등급 아이템 나올 확률 2배, 아이템 2개 나올 확률 2배";
                        break;
                }
                UpgradeDisplayers[i].text =  str;
                
                UpgradeCostDisplayers[i].text = dataDic.FindUpgrade(upgradeIndex).cost[currentUpgradeLV[i]] + "골드";
            }

        }
    }


    public void Upgrade(int upgradeIndex)//아래 두 함수 하나로, 실패 구현
    {
        int id = upgradeIndex - 50001;
        if (IsMaxUpgraded(upgradeIndex))
        {
            Debug.Log("Level Max");
            return;
        }
        //Debug.Log("Upgrade index: " + upgradeIndex);
        float prob= (successRate[currentUpgradeLV[id]] / 100f);
        
        if (DataController.Instance.Gold < (ulong)dataDic.FindUpgrade(upgradeIndex).cost[currentUpgradeLV[id]])
        {
            PopUpWindow.Alert("골드가 부족해요.", this);
            return;
        }
        UnityEngine.Random.InitState((int)Time.time);
        PopUpWindow.SetSliderValue(prob);
        DataController.Instance.Gold -= (ulong)dataDic.FindUpgrade(upgradeIndex).cost[currentUpgradeLV[id]];
        if (UnityEngine.Random.value<prob)//업그레이드 레벨은 0~20이고 20에선 업글 불가
        {
            Action onComplete = () => PopUpWindow.Alert("업그레이드 성공!", this);
            onComplete += () => PopUpWindow.HideSlider();
            PopUpWindow.AnimateSlider(1, 0.6f, this, onComplete);
            DataController.upgradeLV.LevelUp(id);
            foreach (GameObject target in dataController.Observers)//관찰자들에게 이벤트 메세지 송출
            {
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick<UpgradeManager>(this, id));
            }

            
        }
        else
        {
            Action onComplete =()=> PopUpWindow.Alert("업그레이드 실패...", this);
            onComplete+= ()=>PopUpWindow.HideSlider();
            PopUpWindow.AnimateSlider(0, 0.6f, this, onComplete);
        }
    }

    public void RemoveAlert()// 테스트 중. "다음 업그레이드"가 0인 업그레이드를 선택했을 때 작동해야함.
    {
        DataController.Instance.NewUpgrade=false;
    }

    private bool IsMaxUpgraded(int upgradeIndex)
    {
        if (upgradeIndex == 50012)
        {
            return currentUpgradeLV[upgradeIndex] >= LAST_UPGRADE_MAX_LEVEL;
        }
        else
        {
            return currentUpgradeLV[upgradeIndex] >= MAX_LEVEL;
        }
    }
}
