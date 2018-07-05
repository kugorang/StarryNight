﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public struct SetItemInfo
    {
        public readonly int Index1, Index2, Index3, Index4, Result;

        public SetItemInfo(int index1, int index2, int index3, int index4, int result)
        {
            Index1 = index1;
            Index2 = index2;
            Index3 = index3;
            Index4 = index4;
            Result = result;
        }
    }

    public struct UpgradeInfo
    {
        public int Index;
        public string Name;
        public int[] Value;
        public int[] Cost;
    }

    public struct TextInfo
    {
        public readonly string Name, Dialogue, Face, Sound;

        public TextInfo(string name, string dialogue, string face, string sound)
        {
            Name = name;
            Dialogue = dialogue;
            Face = face;
            Sound = sound;
        }
    }

    public class DataDictionary : MonoBehaviour
    {
        private static DataDictionary _instance;

        public List<int> FirstQuestsOfScene = new List<int>();

        /// <summary>
        ///     NOTE: 재료를 찾을 때 사용하는 Dictionary
        ///     <para> -> key(int) : 재료 기준표 인덱스</para>
        ///     <para> -> value(ItemInfo) : 재료 정보</para>
        /// </summary>
        public Dictionary<int, ItemInfo> FindItemDic { get; private set; }

        /// <summary>
        ///     NOTE : 재료 조합식 Dictionary
        ///     <para>-> key(int) : material1 인덱스</para>
        ///     <para>-> value(int) : material1에 해당하는 조합식 list</para>
        /// </summary>
        private Dictionary<Tuple<int, int>, List<int>> CombineDic { get; set; }

        /// <summary>
        ///     NOTE: 세트 아이템 조합식을 저장하는 Dictionary
        ///     <para>-> key(int) : </para>
        /// </summary>
        [HideInInspector]
        public List<SetItemInfo> SetCombineList { get; private set; }

        /// <summary>
        ///     NOTE: 퀘스트를 찾을 때 사용하는 Dictionary
        ///     <para> -> key(int) : 퀘스트 기준표 인덱스</para>
        ///     <para> -> value(ItemInfo) : 퀘스트 정보</para>
        /// </summary>
        public Dictionary<int, QuestInfo> FindQuestDic { get; private set; }

        /// <summary>
        ///     NOTE: 업그레이드를 찾을 때 사용하는 Dictionary
        ///     <para> -> key(int) : 업그레이드 기준표 인덱스</para>
        ///     <para> -> value(ItemInfo) : 업그레이드 정보</para>
        /// </summary>
        public Dictionary<int, UpgradeInfo> FindUpDic { get; private set; }

        // 대화 Dictionary
        // Key : 인덱스, Value : 대화 정보
        public Dictionary<int, TextInfo> DialogueDic { get; private set; }

        public int StarNum { get; private set; }
        public int MaterialNum { get; private set; }
        public int CombineNum { get; private set; }

        public static DataDictionary Instance
        {
            get
            {
                if (_instance != null) 
                    return _instance;
                
                _instance = FindObjectOfType<DataDictionary>();

                if (_instance != null) 
                    return _instance;
                
                var container = new GameObject("DataDictionary");
                
                _instance = container.AddComponent<DataDictionary>();

                return _instance;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(this);

            // 두 Dictionary들을 초기화
            FindItemDic = new Dictionary<int, ItemInfo>();
            CombineDic = new Dictionary<Tuple<int, int>, List<int>>();
            SetCombineList = new List<SetItemInfo>();
            FindQuestDic = new Dictionary<int, QuestInfo>();
            FindUpDic = new Dictionary<int, UpgradeInfo>();
            DialogueDic = new Dictionary<int, TextInfo>();

            // 읽어들이기
            ReadDataFile("dataTable/combineTable", Fileinfo.Combinetable);
            ReadDataFile("dataTable/itemTable", Fileinfo.Itemtable);
            ReadDataFile("dataTable/setItemTable", Fileinfo.Setitemtable);
            ReadDataFile("dataTable/questTable", Fileinfo.Questtable);
            ReadDataFile("dataTable/upgradeTable", Fileinfo.Upgradetable);
            ReadDataFile("dataTable/dialogue", Fileinfo.Dialoguetable);
        }

        private void ReadDataFile(string fileName, Fileinfo fileType)
        {
            var txtFile = (TextAsset) Resources.Load(fileName);
            var lineList = txtFile.text.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

            var lineListLen = lineList.Length;

            var formerSceneName = "";

            for (var i = 0; i < lineListLen; i++)
            {
                var wordList = lineList[i].Split(',', '\t');
                int index;
                
                switch (fileType)
                {
                    case Fileinfo.Combinetable:
                        var material1 = Convert.ToInt32(wordList[0]);
                        var material2 = Convert.ToInt32(wordList[1]);
                        var resultNum = Convert.ToInt32(wordList[2]);

                        Tuple<int, int> tuple;

                        if (material1 != material2)
                        {
                            tuple = new Tuple<int, int>(material2, material1);
                            CombineDic[tuple] = new List<int>();

                            for (var j = 0; j < resultNum; j++) 
                                CombineDic[tuple].Add(Convert.ToInt32(wordList[3 + j]));
                        }

                        tuple = new Tuple<int, int>(material1, material2);
                        CombineDic[tuple] = new List<int>();

                        for (var j = 0; j < resultNum; j++) 
                            CombineDic[tuple].Add(Convert.ToInt32(wordList[3 + j]));

                        break;
                    case Fileinfo.Itemtable:
                        index = Convert.ToInt32(wordList[0]);
                        var sellPrice = Convert.ToInt32(wordList[5]);
                        var group = wordList[2];

                        switch (group)
                        {
                            case "별":
                                StarNum++;
                                break;
                            case "재료":
                                MaterialNum++;
                                break;
                            case "아이템":
                                CombineNum++;
                                break;
                        }

                        FindItemDic[index] = new ItemInfo(index, wordList[1], group, wordList[3], sellPrice, wordList[4],
                            "itemImg/item_" + index);

                        break;
                    case Fileinfo.Setitemtable:
                        var setItemInfo = new SetItemInfo(Convert.ToInt32(wordList[0]), Convert.ToInt32(wordList[1]),
                            Convert.ToInt32(wordList[2]), Convert.ToInt32(wordList[3]), Convert.ToInt32(wordList[4]));

                        SetCombineList.Add(setItemInfo);

                        break;
                    case Fileinfo.Questtable:
                        index = Convert.ToInt32(wordList[0]);
                        //int dialogueStart = Convert.ToInt32(wordList[1]);
                        //int dialogueEnd = Convert.ToInt32(wordList[2]);

                        var termsItem = Convert.ToInt32(wordList[4]);
                        var termsCount = Convert.ToInt32(wordList[5]);
                        var reward = Convert.ToInt32(wordList[6]);
                        var rewardCount = Convert.ToInt32(wordList[7]);

                        FindQuestDic[index] = new QuestInfo(index, wordList[1], wordList[2], wordList[3], termsItem,
                            termsCount, reward, rewardCount);


                        if (wordList[1] != formerSceneName) //씬의 첫 퀘스트의 인덱스 구하기
                        {
                            FirstQuestsOfScene.Add(index);
                            formerSceneName = wordList[1];
                        }

                        break;
                    case Fileinfo.Upgradetable:
                        index = Convert.ToInt32(wordList[0]);
                        var len = (wordList.Length - 2) / 2; //wordList의 앞 두 개는 각각 이름과 설명이므로 -2, 그리고 (효과,값)쌍이므로 /2
                        UpgradeInfo upInfo;
                        upInfo.Index = index;
                        upInfo.Name = wordList[1];

                        upInfo.Value = new int[len];
                        upInfo.Cost = new int[len];

                        for (var j = 0; j < len; j++)
                        {
                            var value = Convert.ToInt32(wordList[2 * j + 2]);
                            var cost = Convert.ToInt32(wordList[2 * j + 3]);
                            upInfo.Value[j] = value;
                            upInfo.Cost[j] = cost;
                        }

                        FindUpDic[index] = upInfo;
                        break;
                    case Fileinfo.Dialoguetable:
                        index = Convert.ToInt32(wordList[0]);

                        // wordList[1] : name, wordList[2] : dialogue, wordList[3] : face, wordList[4] : sound
                        DialogueDic[index] = new TextInfo(wordList[1], wordList[2], wordList[3], wordList[4]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("fileType", fileType, null);
                }
            }
        }

        /// <summary>
        ///     기준표에서 아이템을 찾는 함수
        /// </summary>
        /// <param name="key">findDic의 key값</param>
        /// <returns>리턴값</returns>
        public ItemInfo FindItem(int key)
        {
            return FindItemDic[key];
        }

        /// <summary>
        ///     조합표에서 검색하는 함수
        /// </summary>
        /// <param name="key1">재료1의 인덱스</param>
        /// <param name="key2">재료2의 인덱스</param>
        /// <returns>찾았다면 결과를, 아니라면 0을 반환</returns>
        public List<int> FindCombine(int key1, int key2)
        {
            var tuple = new Tuple<int, int>(key1, key2);

            return CombineDic.ContainsKey(tuple) ? CombineDic[tuple] : null;
        }

        public SetItemInfo CheckSetItemCombine(int key)
        {
            var dataController = DataController.Instance;
            var haveDic = dataController.HaveDic;

            foreach (var setItemInfo in SetCombineList)
                if (key == setItemInfo.Index1)
                {
                    if (haveDic.ContainsKey(setItemInfo.Index2) && haveDic[setItemInfo.Index2].Count > 0 
                        && haveDic.ContainsKey(setItemInfo.Index3) && haveDic[setItemInfo.Index3].Count > 0 
                        && haveDic.ContainsKey(setItemInfo.Index4) && haveDic[setItemInfo.Index4].Count > 0) 
                        return setItemInfo;
                }
                else if (key == setItemInfo.Index2)
                {
                    if (haveDic.ContainsKey(setItemInfo.Index1) && haveDic[setItemInfo.Index1].Count > 0 
                        && haveDic.ContainsKey(setItemInfo.Index3) && haveDic[setItemInfo.Index3].Count > 0 
                        && haveDic.ContainsKey(setItemInfo.Index4) && haveDic[setItemInfo.Index4].Count > 0) 
                        return setItemInfo;
                }
                else if (key == setItemInfo.Index3)
                {
                    if (haveDic.ContainsKey(setItemInfo.Index1) && haveDic[setItemInfo.Index1].Count > 0 
                        && haveDic.ContainsKey(setItemInfo.Index2) && haveDic[setItemInfo.Index2].Count > 0 
                        && haveDic.ContainsKey(setItemInfo.Index4) && haveDic[setItemInfo.Index4].Count > 0) 
                        return setItemInfo;
                }
                else if (key == setItemInfo.Index4)
                {
                    if (haveDic.ContainsKey(setItemInfo.Index1) && haveDic[setItemInfo.Index1].Count > 0 
                        && haveDic.ContainsKey(setItemInfo.Index2) && haveDic[setItemInfo.Index2].Count > 0 
                        && haveDic.ContainsKey(setItemInfo.Index3) && haveDic[setItemInfo.Index3].Count > 0) 
                        return setItemInfo;
                }

            return new SetItemInfo(0, 0, 0, 0, 0);
        }

        /// <summary>
        ///     기준표에서 퀘스트를 찾는 함수
        /// </summary>
        /// <param name="key">findQuestDic의 key값</param>
        /// <returns>리턴값</returns>
        public QuestInfo FindQuest(int key)
        {
            return FindQuestDic[key];
        }

        public UpgradeInfo FindUpgrade(int key)
        {
            return FindUpDic[key];
        }

        private enum Fileinfo
        {
            Combinetable,
            Itemtable,
            Setitemtable,
            Questtable,
            Upgradetable,
            Dialoguetable
        }

        private struct Tuple<T1, T2>
        {
            public readonly T1 Item1;
            public readonly T2 Item2;

            public Tuple(T1 item1, T2 item2)
            {
                Item1 = item1;
                Item2 = item2;
            }
        }
    }
}