using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestDictionary : MonoBehaviour {

    enum FILEINFO
    {
        QUESTTABLE
    }

    /// <summary>
    /// NOTE: 퀘스트를 찾을 때 사용하는 Dictionary
    /// <para> -> key(int) : 퀘스트 기준표 인덱스</para>
    /// <para> -> value(ItemInfo) : 퀘스트 정보</para>
    /// </summary>
    public Dictionary<int, QuestInfo> findQuestDic;

    private void Start()
    {
        // Dictionary을 초기화
        findQuestDic = new Dictionary<int, QuestInfo>();

        ReadDataFile("dataTable/questTable", FILEINFO.QUESTTABLE);
    }

    private void ReadDataFile(string fileName, FILEINFO fileType)
    {
        TextAsset txtFile = (TextAsset)Resources.Load(fileName) as TextAsset;
        string txt = txtFile.text;
        string[] stringOperators = new string[] { "\r\n" };
        string[] lineList = txt.Split(stringOperators, StringSplitOptions.None);

        int lineListLen = lineList.Length;

        for (int i = 0; i < lineListLen; i++)
        {
            string[] wordList = lineList[i].Split(',');

            int index = Convert.ToInt32(wordList[0]);
            int termsItem = Convert.ToInt32(wordList[4]);
            int termsCount = Convert.ToInt32(wordList[5]);
            int reward = Convert.ToInt32(wordList[6]);
            int rewardCount = Convert.ToInt32(wordList[7]);

            findQuestDic[index] = gameObject.AddComponent<QuestInfo>();
            findQuestDic[index].Init(index, wordList[1], wordList[2], wordList[3], termsItem, termsCount, reward, rewardCount);
        }
    }

    /// <summary>
    /// 기준표에서 퀘스트를 찾는 함수
    /// </summary>
    /// <param name="key">findQuestDic의 key값</param>
    /// <returns>리턴값</returns>
    public QuestInfo FindQuest(int key)
    {
        return findQuestDic[key];
    }
}
