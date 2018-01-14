using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInfo : MonoBehaviour {

    // 퀘스트 기준 표 index
    public int index { get; set; }

    // 퀘스트 발생 액트
    public string act { get; set; }

    // 퀘스트 제목
    public string title { get; set; }

    // 퀘스트 내용
    public string content { get; set; }

    // 퀘스트 조건 아이템
    public int termsItem { get; set; }

    // 퀘스트 조건 아이템 갯수
    public int termsCount { get; set; }

    // 퀘스트 보상 아이템
    public int reward { get; set; }

    // 퀘스트 보상 아이템 갯수
    public int rewardCount { get; set; }

    public void Init(int _index, string _act, string _title, string _content, int _termsItem, int _termsCount, int _reward, int _rewardCount)
    {
        index = _index;
        act = _act;
        title = _title;
        content = _content;
        termsItem = _termsItem;
        termsCount = _termsCount;
        reward = _reward;
        rewardCount = _rewardCount;
    }
}
