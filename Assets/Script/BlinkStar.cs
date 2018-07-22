using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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

        #region ShowingIndex;

        private static int FirstQuest;


        private static int LastQuest; // 원래는 90247

        private static int _showingQuestIndex;

       /* private static readonly Dictionary<int, string> CurrentSceneName = new Dictionary<int, string>()
        {
            {0,"Aries"},
            {1,"Taurus"}
        };*/

        public static int ShowingQuestIndex
        {
            get { return _showingQuestIndex; }
            private set
            {
                if (value < FirstQuest)
                    _showingQuestIndex = FirstQuest;
                else if (value > LastQuest)
                    _showingQuestIndex = LastQuest;
                else
                    _showingQuestIndex = value;
            }
        }
        #endregion

       

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

            _ownQuest = _dataDic.FindQuestDic[QuestIndex];

            FirstQuest = _dataDic.FirstQuestsOfScene.First();
            LastQuest = _dataDic.LastQuestsOfScene.Last();
            /*_ownQuest = new QuestInfo(QuestIndex, findQuestInfo.Act, findQuestInfo.DialogueStartIndex,
                findQuestInfo.DialogueEndIndex, findQuestInfo.Title, findQuestInfo.Content,
                findQuestInfo.TermsNum, findQuestInfo.RewardNum);*/

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
            if (ShowingQuestIndex >= FirstQuest) 
                return;
            
            ShowingQuestIndex = Quest.Progress;
                
            if (Quest.Progress == QuestIndex)
                ShowQuestInfo();
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

            ShowingQuestIndex = QuestIndex;
            ShowQuestInfo();

            // 클릭 메시지를 다른 별들에게 송출
            var stars = GameObject.FindGameObjectsWithTag("BlinkStars");
            
            foreach (var target in stars) 
                ExecuteEvents.Execute<IClickables>(target, null, (x, y) => x.OnOtherClick());
        }

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
            else if (Quest.Progress < 90212) 
            {                
                var nextStar = GameObject.Find("Taurus_" + Quest.Progress).GetComponent<BlinkStar>();
                nextStar.BlingBling();
            }
            // 쌍둥이자리
            else if (Quest.Progress < 90318) 
            {                
                var nextStar = GameObject.Find("Gemini_" + Quest.Progress).GetComponent<BlinkStar>();
                nextStar.BlingBling();
            }
            // 게자리
            else if (Quest.Progress < 90406) 
            {                
                var nextStar = GameObject.Find("Cancer_" + Quest.Progress).GetComponent<BlinkStar>();
                nextStar.BlingBling();
            }
        }

        private void ShowQuestInfo()
        {
            // 퀘스트 제목 출력
            GameObject.Find("Name Displayer").GetComponent<Text>().text = _ownQuest.Title;

            // 퀘스트 진행상태 출력
            switch (QuestIndex)
            {
                case 90101:    // 별의 원석 아무거나 1개 획득
                {
                    int[] itemIndex = { 1001, 1006, 1011 };
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));

                    GameObject.Find("Progress Displayer").GetComponent<Text>().text 
                        = QuestIndex < Quest.Progress ? "퀘스트 완료" : string.Format("별의 원석 {0} / 1", questItemNum);
                    break;
                }
                case 90102:    // 재료 아이템 아무거나 1개 획득
                {
                    int[] itemIndex = { 2001, 2006, 2011, 2016, 2021, 2026 };
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));
                    
                    GameObject.Find("Progress Displayer").GetComponent<Text>().text 
                        = QuestIndex < Quest.Progress ? "퀘스트 완료" : string.Format("재료 아이템 {0} / 1", questItemNum);
                    break;
                }
                case 90103:    // 별의 파편 아무거나 1개 획득
                {
                    int[] itemIndex = { 1002, 1007, 1012 };
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));

                    GameObject.Find("Progress Displayer").GetComponent<Text>().text 
                        = QuestIndex < Quest.Progress ? "퀘스트 완료" : string.Format("별의 파편 {0} / 1", questItemNum);
                    break;
                }
                case 90202:    // 세트 아이템 재료 1개 획득 성공
                {
                    int[] itemIndex = { 
                        4001, 4002, 4003, 4004, 4006, 4007, 4008,	4009,
                        4011, 4012, 4013, 4014, 4016, 4017, 4018, 4019,
                        4021, 4022,	 4023, 4024, 4026, 4027, 4028, 4029,
                        4031, 4032,	 4033, 4034, 4036, 4037, 4038, 4039,
                        4041, 4042,	 4043, 4044, 4046, 4047, 4048, 4049,
                        4051, 4052, 4053, 4054, 4056, 4057, 4058
                    };
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));

                    GameObject.Find("Progress Displayer").GetComponent<Text>().text 
                        = QuestIndex < Quest.Progress ? "퀘스트 완료" : string.Format("세트 아이템 재료 {0} / 1", questItemNum);
                    break;
                }
                default:
                    GameObject.Find("Progress Displayer").GetComponent<Text>().text = "";
                    
                    if (QuestIndex < Quest.Progress)
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                    else
                    {
                        var isTermFirst = true;
                        
                        foreach (var term in _ownQuest.TermsDic)
                        {
                            var termItemIndex = term.Key;
                    
                            if (termItemIndex == 9999)     // 조건이 골드일 때
                            {
                                GameObject.Find("Progress Displayer").GetComponent<Text>().text += isTermFirst 
                                    ? string.Format("골드 {0} / {1}", _dataController.Gold, term.Value) 
                                    : string.Format("\n골드 {0} / {1}", _dataController.Gold, term.Value);
                            }
                            else if (termItemIndex > 50000) // 조건이 업그레이드일 때
                            {
                                GameObject.Find("Progress Displayer").GetComponent<Text>().text += isTermFirst 
                                    ? string.Format("{0} {1} / {2}", _dataDic.FindUpgrade(termItemIndex).Name, DataController.UpgradeLv[termItemIndex], term.Value) 
                                    : string.Format("\n{0} {1} / {2}", _dataDic.FindUpgrade(termItemIndex).Name, DataController.UpgradeLv[termItemIndex], term.Value);
                            }
                            else
                            {
                                GameObject.Find("Progress Displayer").GetComponent<Text>().text += isTermFirst 
                                    ? string.Format("{0} {1} / {2}", _dataDic.FindItemDic[termItemIndex].Name, _dataController.GetItemNum(termItemIndex), term.Value) 
                                    : string.Format("\n{0} {1} / {2}", _dataDic.FindItemDic[termItemIndex].Name, _dataController.GetItemNum(termItemIndex), term.Value);
                            }
                            
                            isTermFirst = false;
                        }
                    }

                    break;
            }

            // 퀘스트 내용 출력
            GameObject.Find("Content Displayer").GetComponent<Text>().text = _ownQuest.Content;
            
            // 퀘스트 보상 출력
            GameObject.Find("Reward Displayer").GetComponent<Text>().text = "";
            var isRewardFirst = true;
            
            foreach (var reward in _ownQuest.RewardDic)
            {
                var rewardIndex = reward.Key;
            
                if (rewardIndex == 9999)
                    GameObject.Find("Reward Displayer").GetComponent<Text>().text += isRewardFirst 
                        ? string.Format("골드 {0}", reward.Value) : string.Format("\n골드 {0}", reward.Value);
                else if (rewardIndex > 50000)
                    GameObject.Find("Reward Displayer").GetComponent<Text>().text += isRewardFirst 
                        ? string.Format("{0} Lv. {1} 해제", _dataDic.FindUpgrade(rewardIndex).Name, reward.Value) 
                        : string.Format("\n{0} Lv. {1} 해제", _dataDic.FindUpgrade(rewardIndex).Name, reward.Value);
                else
                    GameObject.Find("Reward Displayer").GetComponent<Text>().text += isRewardFirst 
                        ? string.Format("{0} {1}", _dataDic.FindItem(rewardIndex).Name, reward.Value) 
                        : string.Format("\n{0} {1}", _dataDic.FindItem(rewardIndex).Name, reward.Value);

                isRewardFirst = false;
            }
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