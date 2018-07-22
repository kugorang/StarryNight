using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script
{
    public class QuestInfo
    {
        // 퀘스트 기준 표 index
        public int Index { get; private set; }

        // 퀘스트 발생 액트
        public string Act { get; private set; }
        
        // 퀘스트 다이얼로그 시작 인덱스
        public int DialogueStartIndex { get; private set; }
        
        // 퀘스트 다이얼로그 끝 인덱스
        public int DialogueEndIndex { get; private set; }
        
        // 퀘스트 제목
        public string Title { get; private set; }

        // 퀘스트 내용
        public string Content { get; private set; }

        // 퀘스트 조건 리스트
        public List<KeyValuePair<int, int>> TermsDic { get; private set; }

        // 퀘스트 보상 리스트
        public List<KeyValuePair<int, int>> RewardDic { get; private set; }
        
        // 생성자
        public QuestInfo(int index, string act, int dialogueStartIndex, int dialogueEndIndex, 
            string title, string content)
        {
            Index = index;
            Act = act;
            DialogueStartIndex = dialogueStartIndex;
            DialogueEndIndex = dialogueEndIndex;
            Title = title;
            Content = content;

            TermsDic = new List<KeyValuePair<int, int>>();
            RewardDic = new List<KeyValuePair<int, int>>();
        }

        public void SetTermsDic(int index, int num)
        {
            TermsDic.Add(new KeyValuePair<int, int>(index, num));
        }
        
        public void SetRewardDic(int index, int num)
        {
            RewardDic.Add(new KeyValuePair<int, int>(index, num));
        }
    }

    public static class Quest
    {
        #region Zodiac

        public const int NumberOfZodiac = 12;

        public enum Zodiac //이후 인덱스 변경 또는 추가에 대비해, 각 별자리의 인덱스를 Enum으로 만듦.
        {
            Aries = 90100,
            Taurus = 90200,
            Gemini = 90300,
            Cancer = 90400,
            Leo = 90500,
            Virgo = 90600,
            Libra = 90700,
            Scorpio = 90800,
            Sagittarius = 90900,
            Capricorn = 91000,
            Aquarius = 91100,
            Pisces = 91200
        }

        /// <summary>
        ///  별자리의 인덱스를 9nn00형식으로 반환.
        /// </summary>
        /// <param name="zodiac"></param>
        /// <returns>sceneName의 int 값</returns>
        public static int GetZodiacIndex(Zodiac zodiac)
        {
            return (int) zodiac;
        }

        /// <summary>
        /// 퀘스트 인덱스에 맞는 별자리 반환
        /// </summary>
        /// <param name="questIndex">퀘스트 인덱스</param>
        /// <returns>별자리</returns>
        public static Zodiac GetZodiacByIndex(int questIndex)
        {
            return (Zodiac)(questIndex - (questIndex % 100));
        }
        /// <summary>
        /// 퀘스트 인덱스를 받아 그 별자리의 순서를 반환(0~11).
        /// </summary>
        /// <param name="questIndex"></param>
        /// <returns>별자리 순서</returns>
        private static int ZodiacIndex(int questIndex)
        {
            return ((questIndex / 100) % 100) - 1;
        }

        #endregion
   
        #region Progress

        public static int Progress
        {
            get { return PlayerPrefs.GetInt("QuestProgress", 90101); }
            private set
            {
                PlayerPrefs.SetInt("QuestProgress", value);
            }
        }

        /// <summary>
        ///     다음 퀘스트로 넘어가기
        /// </summary>
        public static void NextQuest()
        {
            var currentIndex = Progress;
            
            // +1 한 퀘스트 인덱스가 있으면 +1 이 다음 퀘스트 인덱스
            if (DataDictionary.Instance.FindQuestDic.ContainsKey(currentIndex + 1)) 
            {
                Progress += 1;
            }
            else // 아니면 다음 별자리로 넘어감.
            {
                Progress = DataDictionary.Instance.FirstQuestsOfScene[ZodiacIndex(currentIndex) + 1];
                  CameraController.AddScene("QuestList");
            }
        }

        public static void ProgressReset()
        {
            Progress = 90101;
        }

        #endregion

        public static bool CheckQuestClear(int index)
        {
            var dataController = DataController.Instance;
            var currentQuest = DataDictionary.Instance.FindQuest(Progress);
            var isClear = true;

            foreach (var term in currentQuest.TermsDic)
            {
                // 진행중인 퀘스트 조건 아이템의 인덱스
                var checkItemIndex = term.Key;
    
                // 진행중인 퀘스트 조건을 만족하는 아이템 개수 
                var checkItemCount = term.Value;
    
                // 현재 가지고 있는 조건 아이템 갯수
                var currentItemNum = 0;
    
                // 조건이 [골드/업그레이드/아이템] 인지 확인
                if (checkItemIndex == 9999) // 골드일 때
                {
                    currentItemNum = (int) dataController.Gold;
                }
                else if (checkItemIndex > 50000) // 업그레이드일 때
                {
                    currentItemNum = UpgradeManager.GetUpgradeLV(checkItemIndex);
                }
                else // 아이템일 때
                {
                    int[] itemIndex;
    
                    switch (currentQuest.Index)
                    {
                        case 90101:    // 별의 원석 아무거나 1개 획득
                            itemIndex = new[] { 1001, 1006, 1011 };
                            currentItemNum += itemIndex.Sum(i => dataController.GetItemNum(i));
                            break;
                        case 90102:    // 재료 아이템 아무거나 1개 획득
                            itemIndex = new[] { 2001, 2006, 2011, 2016, 2021, 2026 };
                            currentItemNum += itemIndex.Sum(i => dataController.GetItemNum(i));
                            break;
                        case 90103:    // 별의 파편 아무거나 1개 획득
                            itemIndex = new[] { 1002, 1007, 1012 };
                            currentItemNum += itemIndex.Sum(i => dataController.GetItemNum(i));
                            break;
                        case 90202:    // 퀘스트 인덱스 90202의 경우
                            itemIndex = new[] 
                            { 
                                4001, 4002, 4003, 4004, 4006, 4007, 4008,	4009,
                                4011, 4012, 4013, 4014, 4016, 4017, 4018, 4019,
                                4021, 4022, 4023, 4024, 4026, 4027, 4028, 4029,
                                4031, 4032, 4033, 4034, 4036, 4037, 4038, 4039,
                                4041, 4042, 4043, 4044, 4046, 4047, 4048, 4049,
                                4051, 4052, 4053, 4054, 4056, 4057, 4058
                            };
                            currentItemNum += itemIndex.Sum(i => dataController.GetItemNum(i));
                            break;
                        default:
                            currentItemNum = dataController.GetItemNum(checkItemIndex);
                            break;
                    }
                }
                
                // 조건 아이템의 갯수 확인 및 보상 지급
                if (checkItemCount <= currentItemNum) 
                    continue;
                
                isClear = false;
                break;
            }

            if (!isClear)
                return false;

            foreach (var reward in currentQuest.RewardDic)
            {
                // 보상이 골드일 때
                if (reward.Key == 9999) 
                {
                    dataController.Gold += (ulong)reward.Value;
                }
                else
                {
                    /*// 아이템 인벤토리가 꽉 차있어도 준다.
                    if (dataController.ItemCount >= dataController.ItemLimit) 
                        return true;*/
                
                    // 보상이 업그레이드 오픈일 때
                    if (reward.Key > 50000) 
                    {
                        // 조건이 골드일 때 골드 감소
                        DecreseGold(currentQuest);

                        // 업그레이드가 순차적으로 열릴 것을 가정한 코드.
                        dataController.LatestUpgradeIndex = reward.Key; 
                    }
                    else
                    {
                        // 조건이 골드일 경우 골드 감소
                        DecreseGold(currentQuest);
                        dataController.InsertNewItem(reward.Key, reward.Value);
                    }
                }
            }
            NextQuest();
            return true;
        }

        private static void DecreseGold(QuestInfo questInfo)
        {
            foreach (var term in questInfo.TermsDic)
            {
                if (term.Key != 9999) 
                    continue;
                
                DataController.Instance.Gold -= (ulong) term.Value;
                return;
            }
        }
    }
}

