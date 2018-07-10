using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace Script
{
    // TODO: quest.cs로 BlinkStar, QuestUIButton로 퀘스트 부분만 떼서 통합 정리
    public class BlinkStar : MonoBehaviour, IClickables
    {
        // 별 깜박임 활성화 유무
        private bool _blinkAlive;

        // 퀘스트 별 버튼
        private Button _btn;
        private Color _btnColorA;
        private Color _btnColorD;

        // 퀘스트 별 버튼 이미지
        private Image _btnImg;

        // 현재 퀘스트
        private DataController _dataController;
        private DataDictionary _dataDic;

        private QuestInfo _ownQuest;
        public int QuestIndex;

        // 퀘스트 진행 상태에 따른 별 이미지 가져오기
        public void OnOtherClick()
        {
            if (QuestIndex > Quest.Progress)
            {
                _btn.enabled = false;
                _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_uncomplete");
            }
            else
            {
                _btn.enabled = true;
                _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_complete");

                if (QuestIndex == Quest.Progress)
                    _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_ongoing");
            }
        }

        private void Awake()
        {
            _dataDic = DataDictionary.Instance;
            _dataController = DataController.Instance;

            _btn = gameObject.GetComponent<Button>();
            _btnImg = gameObject.GetComponent<Image>();
            _btn.enabled = false;

            _btnColorD.a = 0;
            _btnColorA.a = 255;
            _btnColorA.r = 255;
            _btnColorA.g = 255;
            _btnColorA.b = 255;

            _blinkAlive = true;

            var findQuestInfo = _dataDic.FindQuestDic[QuestIndex];
            _ownQuest = new QuestInfo(QuestIndex, findQuestInfo.Act, findQuestInfo.DialogueStartIndex,
                findQuestInfo.DialogueEndIndex, findQuestInfo.Title, findQuestInfo.Content,
                findQuestInfo.TermsItem, findQuestInfo.TermsCount, findQuestInfo.Reward, findQuestInfo.RewardCount);

            // 퀘스트 진행도 확인 후 별 sprite 설정 및 버튼 활성화
            if (QuestIndex > Quest.Progress) // 진행 전 퀘스트
            {
                _btn.enabled = false;
                _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_uncomplete");
            }
            else
            {
                _btn.enabled = true;
                _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_complete");

                // 진행중인 퀘스트
                if (QuestIndex != Quest.Progress) 
                    return;
                
                _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_ongoing");
                StartCoroutine(Blink());
            }
        }

        private void Start()
        {
            if (QuestUIButton.ShowingQuestIndex < 90101 && Quest.Progress == QuestIndex) 
                OnClick();
        }

        // 퀘스트 정보 확인 및 퀘스트 완료 보상 지급
        public void OnClick()
        {
            AudioManager.Instance.QuestStarSound();

            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));

            if (Quest.CheckQuestClear(QuestIndex))
            {
                OnQuestClear();
            }
            else if (_dataController.IsTutorialEnd && _ownQuest.DialogueStartIndex >= 300701)
            {
                _dataController.NowIndex = _ownQuest.DialogueStartIndex;
                DialogueManager.Instance.ShowDialogue();
            }

            QuestUIButton.ShowingQuestIndex = QuestIndex;
            ShowQuestInfo();

            // 클릭 메시지를 다른 별들에게 송출
            var stars = GameObject.FindGameObjectsWithTag("BlinkStars");
            
            foreach (var target in stars) 
                ExecuteEvents.Execute<IClickables>(target, null, (x, y) => x.OnOtherClick());
        }

        private void OnQuestClear()
        {
            Quest.NextQuest();
            _blinkAlive = false;

            // 다음 퀘스트 별 반짝 거리기
            if (Quest.Progress < 90105) // 양자리 일 때
            {
                var nextStar = GameObject.Find("Aries_" + Quest.Progress).GetComponent<BlinkStar>();
                nextStar.BlingBling();
            }
            // 황소자리일 때 수 주의. 나중에 변수나 상수로 쓸 것
            else if (Quest.Progress > 90104 && Quest.Progress < 90124) 
            {                
                var nextStar = GameObject.Find("Taurus_" + Quest.Progress).GetComponent<BlinkStar>();
                nextStar.BlingBling();
            }
        }

        public void ShowQuestInfo()
        {
            // 퀘스트 제목 출력
            GameObject.Find("Name Displayer").GetComponent<Text>().text = _ownQuest.Title;

            switch (QuestIndex)
            {
                // 퀘스트 진행상태 출력
                case 90101:
                {
                    int[] itemIndex = {1001, 1006, 1011};
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));

                    if (QuestIndex < Quest.Progress)
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                    else
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                            "별의 원석 " + questItemNum + "/" + _ownQuest.TermsCount;
                    break;
                }
                case 90102:
                {
                    int[] itemIndex = {2001, 2006, 2011, 2016, 2021, 2026 };
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));
                    
                    if (QuestIndex < Quest.Progress)
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                    else
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                            "재료 아이템 " + questItemNum + "/" + _ownQuest.TermsCount;
                    break;
                }
                case 90103:
                {
                    int[] itemIndex = { 1002, 1007, 1012 };
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));

                    if (QuestIndex < Quest.Progress)
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                    else
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                            "별의 파편 " + questItemNum + "/" + _ownQuest.TermsCount;
                    break;
                }
                    /*
                case 90104:
                {
                    int[] itemIndex = {3001, 3002, 3003, 3016, 3017, 3018, 3031, 3032, 3033, 3046, 3047, 3048, 3061, 3062, 3063, 3076, 3077, 3078};
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));

                    if (QuestIndex < Quest.Progress)
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                    else
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                            "골드 " + questItemNum + "/" + _ownQuest.TermsCount;
                    break;
                }
                */
                default:
                    var termItemIndex = _ownQuest.TermsItem;
                    
                    if (termItemIndex == 9999) // 조건이 골드일 때
                    {
                        if (QuestIndex < Quest.Progress)
                            GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                        else
                            GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                                "골드 " + _dataController.Gold + "/" + _ownQuest.TermsCount;
                    }
                    else if (termItemIndex > 50000) // 조건이 업그레이드일 때
                    {
                        if (QuestIndex < Quest.Progress)
                            GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                        else
                            GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                                _dataDic.FindUpgrade(termItemIndex).Name + DataController.UpgradeLv[termItemIndex] + "/" +
                                _ownQuest.TermsCount;
                    }
                    else
                    {
                        if (QuestIndex < Quest.Progress)
                            GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                        else
                            GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                                _dataDic.FindItemDic[termItemIndex].Name + _dataController.GetItemNum(termItemIndex) + "/" +
                                _ownQuest.TermsCount;
                    }

                    break;
            }

            // 퀘스트 내용 출력
            GameObject.Find("Content Displayer").GetComponent<Text>().text = _ownQuest.Content;

            // 퀘스트 보상 출력
            var rewardIndex = _ownQuest.Reward;
            
            if (rewardIndex == 9999)
                GameObject.Find("Reward Displayer").GetComponent<Text>().text = "골드 " + _ownQuest.RewardCount;
            else if (rewardIndex > 50000)
                GameObject.Find("Reward Displayer").GetComponent<Text>().text =
                    _dataDic.FindUpgrade(rewardIndex).Name + " Lv." + _ownQuest.RewardCount + " 오픈";
            else
                GameObject.Find("Reward Displayer").GetComponent<Text>().text =
                    _dataDic.FindItem(rewardIndex).Name + " " + _ownQuest.RewardCount;
        }

        private void BlingBling()
        {
            StartCoroutine(Blink());
        }

        // 진행중인 별 깜박임
        private IEnumerator Blink()
        {
            while (_blinkAlive)
            {
                yield return new WaitForSeconds(0.5f);
                _btnImg.color = _btnColorD;
                yield return new WaitForSeconds(0.5f);
                _btnImg.color = _btnColorA;
            }
        }
    }
}