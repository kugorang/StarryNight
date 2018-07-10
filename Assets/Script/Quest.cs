using System.Linq;
using UnityEngine;

namespace Script
{
    public class QuestInfo
    {
        public QuestInfo(int index, string act, int dialogueStartIndex, int dialogueEndIndex, 
            string title, string content, int termsItem, int termsCount, int reward, int rewardCount)
        {
            Index = index;
            Act = act;
            DialogueStartIndex = dialogueStartIndex;
            DialogueEndIndex = dialogueEndIndex;
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
        
        // 퀘스트 다이얼로그 시작 인덱스
        public int DialogueStartIndex { get; private set; }
        
        // 퀘스트 다이얼로그 끝 인덱스
        public int DialogueEndIndex { get; private set; }
        
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

    public class Quest : MonoBehaviour
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

        /*
	    /// <summary>
	    /// 이름으로 index 반환
	    /// </summary>
	    /// <param name="zodiac">Name of scene</param>
	    /// <returns></returns>
	    public int GetZodiacIndex(string zodiac)
	    {
	        return (int)Enum.Parse(typeof(Zodiac), zodiac);
	    }
        */
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
            return ((questIndex / 100)%100)-1;
        }

        /*
	    /// <summary>
	    ///    QuestIndex에 맞는 이름 반환.
	    /// </summary>
	    /// <param name="index">퀘스트 인덱스</param>
	    /// <returns></returns>
	    public string GetZodiacName(int index)
	    {
	        return GetZodiacName((Zodiac)index);
	    }

	    /// <summary>
	    ///     Enum.GetName(type, object)와 동일.
	    ///     ZodiacName에 해당하는 이름 반환.
	    /// </summary>
	    /// <param name="zodiac"></param>
	    /// <returns></returns>
	    private static string GetZodiacName(Zodiac zodiac)
	    {
	        return Enum.GetName(typeof(Zodiac), zodiac);
	    }*/

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
            if (DataDictionary.Instance.FindQuestDic.ContainsKey(currentIndex + 1))//+1한 퀘스트 인덱스가 있으면 +1이 다음 퀘스트 인덱스 
            {
                Progress += 1;
            }
            else//아니면 다음 별자리로 넘어감.
            {
                Progress = DataDictionary.Instance.FirstQuestsOfScene[ZodiacIndex(currentIndex)+1];
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
            
            // 진행중인 퀘스트 조건 아이템의 인덱스
            var checkItemIndex = currentQuest.TermsItem;

            // 진행중인 퀘스트 조건을 만족하는 아이템 개수 
            var checkItemCount = currentQuest.TermsCount;

            // 현재 가지고 있는 조건 아이템 갯수
            var currentItemNum = 0;

            // 조건이 [골드/업그레이드/아이템] 인지 확인
            if (checkItemIndex == 9999) // 골드일 때
            {
                currentItemNum = (int) dataController.Gold;
            }
            else if (checkItemIndex > 50000) // 업그레이드일 때
            {
                currentItemNum = DataController.UpgradeLv[checkItemIndex];
            }
            else // 아이템일 때
            {
                int[] itemIndex;

                switch (currentQuest.Index)
                {
                    // 퀘스트 인덱스 90101의 경우
                    case 90101:
                        itemIndex = new[] {1001, 1006, 1011};
                        currentItemNum += itemIndex.Sum(i => dataController.GetItemNum(i));
                        break;
                    // 퀘스트 인덱스 90102의 경우
                    case 90102:
                        itemIndex = new[] {2001, 2006, 2011, 2016, 2021, 2026 };
                        currentItemNum += itemIndex.Sum(i => dataController.GetItemNum(i));
                        break;
                    // 퀘스트 인덱스 90103의 경우
                    case 90103:
                        itemIndex = new[] { 1002, 1007, 1012 };
                        currentItemNum += itemIndex.Sum(i => dataController.GetItemNum(i));
                        break;
                       /*
                    // 퀘스트 인덱스 90104의 경우
                    case 90104:
                        itemIndex = new[]
                        {
                            3001, 3002, 3003, 3016, 3017, 3018, 3031, 3032, 3033, 
                            3046, 3047, 3048, 3061, 3062, 3063, 3076, 3077, 3078
                        };
                        currentItemNum += itemIndex.Sum(i => dataController.GetItemNum(i));
                        break;
                        */
                    default:
                        currentItemNum = dataController.GetItemNum(checkItemIndex);
                        break;
                }
            }

            // 조건 아이템의 갯수 확인 및 보상 지급
            if (checkItemCount > currentItemNum) 
                return false;
            
            // 보상이 골드일 때
            if (currentQuest.Reward == 9999) 
            {
                dataController.Gold += (ulong)currentQuest.RewardCount;
            }
            else
            {
                // 아이템 인벤토리가 꽉 차있는지 확인
                if (dataController.ItemCount >= dataController.ItemLimit) 
                    return true;
                    
                // 보상이 업그레이드 오픈일 때
                if (currentQuest.Reward > 50000) 
                {
                    // 조건이 골드일 때 골드 감소
                    if (currentQuest.TermsItem == 9999) 
                        dataController.Gold -= (ulong)currentQuest.TermsCount;

                    // 업그레이드가 순차적으로 열릴 것을 가정한 코드.
                    dataController.LatestUpgradeIndex = currentQuest.Reward; 
                }
                else
                {
                    // 조건이 골드일 경우 골드 감소
                    if (currentQuest.TermsItem == 9999)
                        dataController.Gold -= (ulong)currentQuest.TermsCount;

                    dataController.InsertNewItem(currentQuest.Reward, currentQuest.RewardCount);
                    //dataController.AddItemCount(); 서적 아이템 인벤토리 차지 안 함;
                }
            }

            return true;
        }

        /*
            public QuestInfo CurrentQuestInfo() { }
            public bool HasQuestFinished;
        */
    }
}

