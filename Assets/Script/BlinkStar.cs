using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestInfo//목표:quest.cs로 BlinkStar, QuestUIButton로 퀘스트 부분만 떼서 통합정리
{
    // 퀘스트 기준 표 index
    public int Index { get; set; }

    // 퀘스트 발생 액트
    public string Act { get; set; }

    // 퀘스트 제목
    public string Title { get; set; }

    // 퀘스트 내용
    public string Content { get; set; }

    // 퀘스트 조건 아이템
    public int TermsItem { get; set; }

    // 퀘스트 조건 아이템 갯수
    public int TermsCount { get; set; }

    // 퀘스트 보상 아이템
    public int Reward { get; set; }

    // 퀘스트 보상 아이템 갯수
    public int RewardCount { get; set; }

    public QuestInfo(int _index, string _act, string _title, string _content, int _termsItem, int _termsCount, int _reward, int _rewardCount)
    {
        Index = _index;
        Act = _act;
        Title = _title;
        Content = _content;
        TermsItem = _termsItem;
        TermsCount = _termsCount;
        Reward = _reward;
        RewardCount = _rewardCount;
    }
}

public class BlinkStar : MonoBehaviour, IClickables
{
    public int questIndex;

    // 퀘스트 별 버튼
    private Button btn;
    // 퀘스트 별 버튼 이미지
    private Image btnImg;

    private Color btnColor_d;
    private Color btnColor_a;

    private DataDictionary dataDic;

    private QuestInfo ownQuest;
    private DataController dataController;
    public DialogueManager dialogueManager;

    // 현재 퀘스트
    private QuestInfo currentQuest;

    // 별 깜박임 활성화 유무
    private bool blinkAlive;

    private void Awake()
    {
        //OnSceneLoaded 활성화 용
        SceneManager.sceneLoaded += OnSceneLoaded;

        dataDic = DataDictionary.Instance;
        dataController = DataController.Instance;

        btn = gameObject.GetComponent<Button>();
        btnImg = gameObject.GetComponent<Image>();
        btn.enabled = false;

        btnColor_d.a = 0;
        btnColor_a.a = 255;
        btnColor_a.r = 255;
        btnColor_a.g = 255;
        btnColor_a.b = 255;

        blinkAlive = true;

        QuestInfo findQuestInfo = dataDic.FindQuestDic[questIndex];
        ownQuest = new QuestInfo(questIndex, findQuestInfo.Act, findQuestInfo.Title, findQuestInfo.Content, findQuestInfo.TermsItem, findQuestInfo.TermsCount, findQuestInfo.Reward, findQuestInfo.RewardCount);
        
        // 퀘스트 진행도 확인 후 별 sprite 설정 및 버튼 활성화

        if (questIndex > dataController.QuestProcess) // 진행 전 퀘스트
        {
            btn.enabled = false;
            btnImg.sprite = Resources.Load<Sprite>("questImg/quest_uncomplete");
        }
        else
        {
            btn.enabled = true;
            btnImg.sprite = Resources.Load<Sprite>("questImg/quest_complete");

            if (questIndex == dataController.QuestProcess) // 진행중인 퀘스트
            {
                btnImg.sprite = Resources.Load<Sprite>("questImg/quest_ongoing");
                StartCoroutine(Blink());
            }
        }
    }

    // 퀘스트 정보 확인 및 퀘스트 완료 보상 지급
    public void OnClick()
    {
        AudioManager.GetInstance().QuestStarSound();
        Debug.Log(dataController.QuestProcess + ", beforefind");
        currentQuest = dataDic.FindQuest(dataController.QuestProcess);
        
        if (dataController.IsTutorialEnd == 0 && dataController.NowIndex == 300135)
        {
            dialogueManager.ContinueDialogue();
        }

        // 진행중인 퀘스트 조건 아이템의 인덱스
        int checkItemIndex = currentQuest.TermsItem;

        // 진행중인 퀘스트 조건을 만족하는 아이템 개수 
        int checkItemCount = currentQuest.TermsCount;
        
        // 현재 가지고 있는 조건 아이템 갯수
        int currentItemNum = 0;

        // 조건이 [골드/업그레이드/아이템] 인지 확인
        if (checkItemIndex == 9999) // 골드일 때
        {
            currentItemNum = (int)dataController.Gold;
        }
        else if (checkItemIndex > 50000) // 업그레이드일 때
        {
            currentItemNum = dataController.CheckUpgradeLevel(checkItemIndex);
        }
        else // 아이템일 때
        {
            if (currentQuest.Index == 90101) // 퀘스트인덱스 90101의 경우
            {
                for (int i = 1001; i < 1004; i++)
                {
                    currentItemNum += dataController.GetItemNum(i);
                }
            }
            else if (currentQuest.Index == 90102) // 퀘스트인덱스 90102의 경우
            {
                for (int i = 1004; i < 1007; i++)
                {
                    currentItemNum += dataController.GetItemNum(i);
                }
            }
            else if (currentQuest.Index == 90103) // 퀘스트인덱스 90103의 경우
            {
                for (int i = 2001; i < 2007; i++)
                {
                    currentItemNum += dataController.GetItemNum(i);
                }
            }
            else if (currentQuest.Index == 90104) // 퀘스트인덱스 90104의 경우
            {
                for (int i = 3001; i < 3019; i++)
                {
                    currentItemNum += dataController.GetItemNum(i);
                }
            }
            else
            {
                currentItemNum = dataController.GetItemNum(checkItemIndex);
            }

        }

        // 조건 아이템의 갯수 확인 및 보상 지급
        if (checkItemCount <= currentItemNum)
        {
            if (currentQuest.Reward == 9999) // 보상이 골드일 때
            {
                dataController.Gold += (ulong)currentQuest.RewardCount;
            }
            else
            {
                // 아이템 인벤토리가 꽉 차있는지 확인
                if (dataController.ItemCount < dataController.ItemLimit)
                {
                    if (currentQuest.Reward > 50000) // 보상이 업그레이드 오픈일 때
                    {
                        if (currentQuest.TermsItem == 9999) // 조건이 골드일 때 골드 감소
                        {
                            dataController.Gold -= (ulong)currentQuest.TermsCount;
                        }

                        dataController.SetMaxUpgradeLevel(currentQuest.Reward);
                    }
                    else
                    {
                        // 조건이 골드일 경우 골드 감소
                        if (currentQuest.TermsItem == 9999)
                        {
                            dataController.Gold -= (ulong)currentQuest.TermsCount;
                        }

                        dataController.InsertNewItem(currentQuest.Reward, currentQuest.RewardCount);
                        //dataController.AddItemCount(); 서적 아이템 인벤토리 차지 안 함;
                    }


                }
            }

            OnQuestClear();
        }

        QuestUIButton.ShowingQuestIndex = questIndex;
        ShowQuestInfo();

        // 클릭 메시지를 다른 별들에게 송출
        GameObject[] stars = GameObject.FindGameObjectsWithTag("BlinkStars");
        foreach (GameObject target in stars)
        {
            ExecuteEvents.Execute<IClickables>(target, null, (x, y) => x.OnOtherClick());
        }
    }

    private void OnQuestClear()
    {
        dataController.NextQuest();
        blinkAlive = false;

        // 다음 퀘스트 별 반짝 거리기
        if (dataController.QuestProcess < 90105) // 양자리 일 때
        {
            if (SceneManager.GetActiveScene().name == "Aries")
            {
                BlinkStar nextStar = GameObject.Find("Aries_" + dataController.QuestProcess).GetComponent<BlinkStar>();
                nextStar.BlingBling();
            }
        }
        else if (dataController.QuestProcess > 90104 && dataController.QuestProcess < 90124) // 황소자리일 때 수 주의.나중에 변수나 상수로 쓸 것
        {
            if (SceneManager.GetActiveScene().name == "Taurus")
            {
                BlinkStar nextStar = GameObject.Find("Taurus_" + dataController.QuestProcess).GetComponent<BlinkStar>();
                nextStar.BlingBling();
            }
        }
    }

    // 퀘스트 진행 상태에 따른 별 이미지 가져오기
    public void OnOtherClick()
    {
        //Debug.Log("Hi");
        if (questIndex > dataController.QuestProcess)
        {
            btn.enabled = false;
            btnImg.sprite = Resources.Load<Sprite>("questImg/quest_uncomplete");
        }
        else
        {
            btn.enabled = true;
            btnImg.sprite = Resources.Load<Sprite>("questImg/quest_complete");

            if (questIndex == dataController.QuestProcess)
            {
                btnImg.sprite = Resources.Load<Sprite>("questImg/quest_ongoing");
            }
        }
    }

    //씬 로드 시 현재 진행 상태로(임시 테스트 함수, 리팩토링 필수)
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log(scene.name);
        if (QuestUIButton.ShowingQuestIndex < 90101 && dataController.QuestProcess == questIndex && gameObject != null)
        {
            OnClick();
        }
    }

    //오브젝트 파괴시 씬로드 함수 비활성화
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ShowQuestInfo()
    {
        // 퀘스트 제목 출력
        GameObject.Find("Name Displayer").GetComponent<Text>().text = ownQuest.Title;

        // 퀘스트 진행상태 출력
        if (questIndex == 90101)
        {
            int questItemNum = 0;
            for (int i = 1001; i < 1004; i++)
            {
                questItemNum += dataController.GetItemNum(i);
            }

            if (questIndex < dataController.QuestProcess)
            {
                //GameObject.Find("Progress Displayer").GetComponent<Text>().text = "별의 원석 " + btn.GetComponent<QuestInfo>().TermsCount + "/" + btn.GetComponent<QuestInfo>().TermsCount;
                GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
            }
            else
            {
                GameObject.Find("Progress Displayer").GetComponent<Text>().text = "별의 원석 " + questItemNum + "/" + ownQuest.TermsCount;
            }
        }
        else if (questIndex == 90102)
        {
            int questItemNum = 0;
            for (int i = 1004; i < 1007; i++)
            {
                questItemNum += dataController.GetItemNum(i);
            }
            if (questIndex < dataController.QuestProcess)
            {
                //GameObject.Find("Progress Displayer").GetComponent<Text>().text = "별의 파편 " + btn.GetComponent<QuestInfo>().TermsCount + "/" + btn.GetComponent<QuestInfo>().TermsCount;
                GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
            }
            else
            {
                GameObject.Find("Progress Displayer").GetComponent<Text>().text = "별의 파편 " + questItemNum + "/" + ownQuest.TermsCount;
            }
        }
        else if (questIndex == 90103)
        {
            int questItemNum = 0;
            for (int i = 2001; i < 2007; i++)
            {
                questItemNum += dataController.GetItemNum(i);
            }

            if (questIndex < dataController.QuestProcess)
            {
                //GameObject.Find("Progress Displayer").GetComponent<Text>().text = "재료 아이템 " + btn.GetComponent<QuestInfo>().TermsCount + "/" + btn.GetComponent<QuestInfo>().TermsCount;
                GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
            }
            else
            {
                GameObject.Find("Progress Displayer").GetComponent<Text>().text = "재료 아이템 " + questItemNum + "/" + ownQuest.TermsCount;
            }
        }
        else if (questIndex == 90104)
        {
            int questItemNum = 0;

            for (int i = 3001; i < 3019; i++)
            {
                questItemNum += dataController.GetItemNum(i);
            }

            if (questIndex < dataController.QuestProcess)
            {
                //GameObject.Find("Progress Displayer").GetComponent<Text>().text = "레벨 1 조합 아이템 " + btn.GetComponent<QuestInfo>().TermsCount + "/" + btn.GetComponent<QuestInfo>().TermsCount;
                GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
            }
            else
            {
                GameObject.Find("Progress Displayer").GetComponent<Text>().text = "레벨 1 조합 아이템 " + questItemNum + "/" + ownQuest.TermsCount;
            }
        }
        else
        {
            int termItemIndex = ownQuest.TermsItem;
            if (termItemIndex == 9999) // 조건이 골드일 때
            {
                if (questIndex < dataController.QuestProcess)
                {
                    //GameObject.Find("Progress Displayer").GetComponent<Text>().text = "골드 " + ownQuest.termsCount + "/" + ownQuest.termsCount;
                    GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                }
                else
                {
                    GameObject.Find("Progress Displayer").GetComponent<Text>().text = "골드 " + dataController.Gold + "/" + ownQuest.TermsCount;
                }
            }
            else if (termItemIndex > 50000) // 조건이 업그레이드일 때
            {

                if (questIndex < dataController.QuestProcess)
                {
                    //GameObject.Find("Progress Displayer").GetComponent<Text>().text = upgradeDic.FindUpgrade(termItemIndex).name + btn.GetComponent<QuestInfo>().TermsCount + "/" + btn.GetComponent<QuestInfo>().TermsCount;
                    GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                }
                else
                {
                    GameObject.Find("Progress Displayer").GetComponent<Text>().text = dataDic.FindUpgrade(termItemIndex).name + dataController.CheckUpgradeLevel(termItemIndex) + "/" + ownQuest.TermsCount;
                }
            }
            else
            {
                if (questIndex < dataController.QuestProcess)
                {
                    //GameObject.Find("Progress Displayer").GetComponent<Text>().text = itemDic.FindItemDic[termItemIndex].Name + btn.GetComponent<QuestInfo>().TermsCount + "/" + btn.GetComponent<QuestInfo>().TermsCount;

                    GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                }
                else
                {
                    GameObject.Find("Progress Displayer").GetComponent<Text>().text = dataDic.FindItemDic[termItemIndex].Name + dataController.GetItemNum(termItemIndex) + "/" + ownQuest.TermsCount;

                }
            }
        }

        // 퀘스트 내용 출력
        GameObject.Find("Content Displayer").GetComponent<Text>().text = ownQuest.Content;

        // 퀘스트 보상 출력
        int rewardIndex = ownQuest.Reward;
        if (rewardIndex == 9999)
        {
            GameObject.Find("Reward Displayer").GetComponent<Text>().text = "골드 " + ownQuest.RewardCount;
        }
        else if (rewardIndex > 50000)
        {
            GameObject.Find("Reward Displayer").GetComponent<Text>().text = dataDic.FindUpgrade(rewardIndex).name + " Lv." + ownQuest.RewardCount + " 오픈";
        }
        else
        {
            GameObject.Find("Reward Displayer").GetComponent<Text>().text = dataDic.FindItem(rewardIndex).Name + " " + ownQuest.RewardCount;
        }
    }

    public void BlingBling()
    {
        StartCoroutine(Blink());
    }

    // 진행중인 별 깜박임
    IEnumerator Blink()
    {
        while (blinkAlive)
        {
            yield return new WaitForSeconds(0.5f);
            btnImg.color = btnColor_d;
            yield return new WaitForSeconds(0.5f);
            btnImg.color = btnColor_a;
        }
    }
}