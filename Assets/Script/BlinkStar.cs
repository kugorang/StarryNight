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
    public class QuestInfo 
    {
        public QuestInfo(int index, string act, string title, string content, 
            int termsItem, int termsCount, int reward, int rewardCount)
        {
            Index = index;
            Act = act;
            Title = title;
            Content = content;
            TermsItem = termsItem;
            TermsCount = termsCount;
            Reward = reward;
            RewardCount = rewardCount;
        }

        // 퀘스트 기준 표 index
        public int Index { get; private set; }

        // 퀘스트 발생 액트
        public string Act { get; private set; }

        // 퀘스트 제목
        public string Title { get; private set; }

        // 퀘스트 내용
        public string Content { get; private set; }

        // 퀘스트 조건 아이템
        public int TermsItem { get; private set; }

        // 퀘스트 조건 아이템 갯수
        public int TermsCount { get; private set; }

        // 퀘스트 보상 아이템
        public int Reward { get; private set; }

        // 퀘스트 보상 아이템 갯수
        public int RewardCount { get; private set; }
    }

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
        private QuestInfo _currentQuest;
        private DataController _dataController;
        private DataDictionary _dataDic;

        private QuestInfo _ownQuest;
        public int QuestIndex;

        // 퀘스트 진행 상태에 따른 별 이미지 가져오기
        public void OnOtherClick()
        {
            if (QuestIndex > _dataController.QuestProcess)
            {
                _btn.enabled = false;
                _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_uncomplete");
            }
            else
            {
                _btn.enabled = true;
                _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_complete");

                if (QuestIndex == _dataController.QuestProcess)
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
            _ownQuest = new QuestInfo(QuestIndex, findQuestInfo.Act, findQuestInfo.Title, findQuestInfo.Content,
                findQuestInfo.TermsItem, findQuestInfo.TermsCount, findQuestInfo.Reward, findQuestInfo.RewardCount);

            // 퀘스트 진행도 확인 후 별 sprite 설정 및 버튼 활성화
            if (QuestIndex > _dataController.QuestProcess) // 진행 전 퀘스트
            {
                _btn.enabled = false;
                _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_uncomplete");
            }
            else
            {
                _btn.enabled = true;
                _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_complete");

                // 진행중인 퀘스트
                if (QuestIndex != _dataController.QuestProcess) 
                    return;
                
                _btnImg.sprite = Resources.Load<Sprite>("questImg/quest_ongoing");
                StartCoroutine(Blink());
            }
        }

        private void Start()
        {
            if (QuestUIButton.ShowingQuestIndex < 90101 && _dataController.QuestProcess == QuestIndex) 
                OnClick();
        }

        // 퀘스트 정보 확인 및 퀘스트 완료 보상 지급
        public void OnClick()
        {
            AudioManager.GetInstance().QuestStarSound();
            _currentQuest = _dataDic.FindQuest(_dataController.QuestProcess);

            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));

            // 진행중인 퀘스트 조건 아이템의 인덱스
            var checkItemIndex = _currentQuest.TermsItem;

            // 진행중인 퀘스트 조건을 만족하는 아이템 개수 
            var checkItemCount = _currentQuest.TermsCount;

            // 현재 가지고 있는 조건 아이템 갯수
            var currentItemNum = 0;

            // 조건이 [골드/업그레이드/아이템] 인지 확인
            if (checkItemIndex == 9999) // 골드일 때
            {
                currentItemNum = (int) _dataController.Gold;
            }
            else if (checkItemIndex > 50000) // 업그레이드일 때
            {
                currentItemNum = DataController.UpgradeLv[checkItemIndex];
            }
            else // 아이템일 때
            {
                int[] itemIndex;
                
                switch (_currentQuest.Index)
                {
                    // 퀘스트 인덱스 90101의 경우
                    case 90101:
                        itemIndex = new[] {1001, 1006, 1011};
                        currentItemNum += itemIndex.Sum(i => _dataController.GetItemNum(i));
                        break;
                    // 퀘스트 인덱스 90102의 경우
                    case 90102:
                        itemIndex = new[] {1002, 1007, 1012};
                        currentItemNum += itemIndex.Sum(i => _dataController.GetItemNum(i));
                        break;
                    // 퀘스트 인덱스 90103의 경우
                    case 90103:
                        itemIndex = new [] {2001, 2006, 2011, 2016, 2021, 2026};
                        currentItemNum += itemIndex.Sum(i => _dataController.GetItemNum(i));
                        break;
                    // 퀘스트 인덱스 90104의 경우
                    case 90104:
                        itemIndex = new[] {3001, 3002, 3003, 3016, 3017, 3018, 3031, 3032, 3033, 3046, 3047, 3048, 3061, 3062, 3063, 3076, 3077, 3078};
                        currentItemNum += itemIndex.Sum(i => _dataController.GetItemNum(i));
                        break;
                    default:
                        currentItemNum = _dataController.GetItemNum(checkItemIndex);
                        break;
                }
            }

            // 조건 아이템의 갯수 확인 및 보상 지급
            if (checkItemCount <= currentItemNum)
            {
                if (_currentQuest.Reward == 9999) // 보상이 골드일 때
                {
                    _dataController.Gold += (ulong) _currentQuest.RewardCount;
                }
                else
                {
                    // 아이템 인벤토리가 꽉 차있는지 확인
                    if (_dataController.ItemCount < _dataController.ItemLimit)
                    {
                        if (_currentQuest.Reward > 50000) // 보상이 업그레이드 오픈일 때
                        {
                            if (_currentQuest.TermsItem == 9999) // 조건이 골드일 때 골드 감소
                                _dataController.Gold -= (ulong) _currentQuest.TermsCount;

                            _dataController.LatestUpgradeIndex = _currentQuest.Reward; //업그레이드가 순차적으로 열릴 것을 가정한 코드.
                        }
                        else
                        {
                            // 조건이 골드일 경우 골드 감소
                            if (_currentQuest.TermsItem == 9999) _dataController.Gold -= (ulong) _currentQuest.TermsCount;

                            _dataController.InsertNewItem(_currentQuest.Reward, _currentQuest.RewardCount);
                            //dataController.AddItemCount(); 서적 아이템 인벤토리 차지 안 함;
                        }
                    }
                }

                OnQuestClear();
            }

            QuestUIButton.ShowingQuestIndex = QuestIndex;
            ShowQuestInfo();

            // 클릭 메시지를 다른 별들에게 송출
            var stars = GameObject.FindGameObjectsWithTag("BlinkStars");
            foreach (var target in stars) ExecuteEvents.Execute<IClickables>(target, null, (x, y) => x.OnOtherClick());
        }

        private void OnQuestClear()
        {
            _dataController.NextQuest();
            _blinkAlive = false;

            // 다음 퀘스트 별 반짝 거리기
            if (_dataController.QuestProcess < 90105) // 양자리 일 때
            {
                if (SceneManager.GetActiveScene().name != "Aries") 
                    return;
                
                var nextStar = GameObject.Find("Aries_" + _dataController.QuestProcess).GetComponent<BlinkStar>();
                nextStar.BlingBling();
            }
            // 황소자리일 때 수 주의. 나중에 변수나 상수로 쓸 것
            else if (_dataController.QuestProcess > 90104 && _dataController.QuestProcess < 90124) 
            {
                if (SceneManager.GetActiveScene().name != "Taurus") 
                    return;
                
                var nextStar = GameObject.Find("Taurus_" + _dataController.QuestProcess).GetComponent<BlinkStar>();
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

                    if (QuestIndex < _dataController.QuestProcess)
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                    else
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                            "별의 원석 " + questItemNum + "/" + _ownQuest.TermsCount;
                    break;
                }
                case 90102:
                {
                    int[] itemIndex = {1002, 1007, 1012};
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));
                    
                    if (QuestIndex < _dataController.QuestProcess)
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                    else
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                            "별의 파편 " + questItemNum + "/" + _ownQuest.TermsCount;
                    break;
                }
                case 90103:
                {
                    int[] itemIndex = {2001, 2006, 2011, 2016, 2021, 2026};
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));

                    if (QuestIndex < _dataController.QuestProcess)
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                    else
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                            "재료 아이템 " + questItemNum + "/" + _ownQuest.TermsCount;
                    break;
                }
                case 90104:
                {
                    int[] itemIndex = {3001, 3002, 3003, 3016, 3017, 3018, 3031, 3032, 3033, 3046, 3047, 3048, 3061, 3062, 3063, 3076, 3077, 3078};
                    var questItemNum = itemIndex.Sum(i => _dataController.GetItemNum(i));

                    if (QuestIndex < _dataController.QuestProcess)
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                    else
                        GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                            "레벨 1 조합 아이템 " + questItemNum + "/" + _ownQuest.TermsCount;
                    break;
                }
                default:
                    var termItemIndex = _ownQuest.TermsItem;
                    
                    if (termItemIndex == 9999) // 조건이 골드일 때
                    {
                        if (QuestIndex < _dataController.QuestProcess)
                            GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                        else
                            GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                                "골드 " + _dataController.Gold + "/" + _ownQuest.TermsCount;
                    }
                    else if (termItemIndex > 50000) // 조건이 업그레이드일 때
                    {
                        if (QuestIndex < _dataController.QuestProcess)
                            GameObject.Find("Progress Displayer").GetComponent<Text>().text = "퀘스트 완료";
                        else
                            GameObject.Find("Progress Displayer").GetComponent<Text>().text =
                                _dataDic.FindUpgrade(termItemIndex).Name + DataController.UpgradeLv[termItemIndex] + "/" +
                                _ownQuest.TermsCount;
                    }
                    else
                    {
                        if (QuestIndex < _dataController.QuestProcess)
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