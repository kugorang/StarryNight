using UnityEngine;

public class QuestInfo : MonoBehaviour
{
    // 퀘스트 기준 표 Index
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

    public void Init(int _index, string _act, string _title, string _content, int _termsItem, int _termsCount, int _reward, int _rewardCount)
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
