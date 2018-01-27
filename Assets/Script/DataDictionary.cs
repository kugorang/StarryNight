using System;
using System.Collections.Generic;
using UnityEngine;

public struct SetItemInfo
{
    public readonly int index1, index2, index3, index4, result;

    public SetItemInfo(int _index1, int _index2, int _index3, int _index4, int _result)
    {
        index1 = _index1;
        index2 = _index2;
        index3 = _index3;
        index4 = _index4;
        result = _result;
    }
}

public struct UpgradeInfo
{
    public int index;
    public string name;
    public int[] value;
    public int[] cost;
}

public class DataDictionary : MonoBehaviour
{

    enum FILEINFO
    {
        COMBINETABLE,
        ITEMTABLE,
        SETITEMTABLE,
        QUESTTABLE,
        UPGRADETABLE
    }

    public struct Tuple<T1, T2>
    {
        public readonly T1 m_item1;
        public readonly T2 m_item2;

        public Tuple(T1 item1, T2 item2)
        {
            m_item1 = item1;
            m_item2 = item2;
        }
    }

    /// <summary>
    /// NOTE: 재료를 찾을 때 사용하는 Dictionary
    /// <para> -> key(int) : 재료 기준표 인덱스</para>
    /// <para> -> value(ItemInfo) : 재료 정보</para>
    /// </summary>
    public Dictionary<int, ItemInfo> FindItemDic { get; private set; }

    /// <summary>
    /// NOTE : 재료 조합식 Dictionary
    /// <para>-> key(int) : material1 인덱스</para>
    /// <para>-> value(int) : material1에 해당하는 조합식 list</para>
    /// </summary>
    public Dictionary<Tuple<int, int>, List<int>> CombineDic { get; private set; }

    /// <summary>
    /// NOTE: 세트 아이템 조합식을 저장하는 Dictionary
    /// <para>-> key(int) : </para>
    /// </summary>
    [HideInInspector]
    public List<SetItemInfo> SetComineList { get; private set; }

    /// <summary>
    /// NOTE: 퀘스트를 찾을 때 사용하는 Dictionary
    /// <para> -> key(int) : 퀘스트 기준표 인덱스</para>
    /// <para> -> value(ItemInfo) : 퀘스트 정보</para>
    /// </summary>
    public Dictionary<int, QuestInfo> FindQuestDic { get; private set; }

    /// <summary>
    /// NOTE: 업그레이드를 찾을 때 사용하는 Dictionary
    /// <para> -> key(int) : 업그레이드 기준표 인덱스</para>
    /// <para> -> value(ItemInfo) : 업그레이드 정보</para>
    /// </summary>
    public Dictionary<int, UpgradeInfo> FindUpDic { get; private set; }

    public int StarNum { get; private set; }
    public int MaterialNum { get; private set; }
    public int CombineNum { get; private set; }

    public List<int> FirstQuestsOfScene = new List<int>();

    private static DataDictionary instance;

    public static DataDictionary Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataDictionary>();

                if (instance == null)
                {
                    GameObject container = new GameObject("DataDictionary");
                    instance = container.AddComponent<DataDictionary>();
                }
            }

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);

        // 두 Dictionary들을 초기화
        FindItemDic = new Dictionary<int, ItemInfo>();
        CombineDic = new Dictionary<Tuple<int, int>, List<int>>();
        SetComineList = new List<SetItemInfo>();
        FindQuestDic = new Dictionary<int, QuestInfo>();
        FindUpDic = new Dictionary<int, UpgradeInfo>();

        // 읽어들이기
        ReadDataFile("dataTable/combineTable", FILEINFO.COMBINETABLE);
        ReadDataFile("dataTable/itemTable", FILEINFO.ITEMTABLE);
        ReadDataFile("dataTable/setItemTable", FILEINFO.SETITEMTABLE);
        ReadDataFile("dataTable/questTable", FILEINFO.QUESTTABLE);
        ReadDataFile("dataTable/upgradeTable", FILEINFO.UPGRADETABLE);
    }

    private void ReadDataFile(string fileName, FILEINFO fileType)
    {
        TextAsset txtFile = (TextAsset)Resources.Load(fileName) as TextAsset;
        string[] lineList = txtFile.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        int lineListLen = lineList.Length;

        string formerSceneName = "";

        for (int i = 0; i < lineListLen; i++)
        {
            string[] wordList = lineList[i].Split(',');
            int index;
            switch (fileType)
            {
                case FILEINFO.COMBINETABLE:
                    int material1 = Convert.ToInt32(wordList[0]);
                    int material2 = Convert.ToInt32(wordList[1]);
                    int resultNum = Convert.ToInt32(wordList[2]);

                    Tuple<int, int> tuple;

                    if (material1 != material2)
                    {
                        tuple = new Tuple<int, int>(material2, material1);

                        CombineDic[tuple] = new List<int>();

                        for (int j = 0; j < resultNum; j++)
                        {
                            CombineDic[tuple].Add(Convert.ToInt32(wordList[3 + j]));
                        }
                    }

                    tuple = new Tuple<int, int>(material1, material2);

                    CombineDic[tuple] = new List<int>();

                    for (int j = 0; j < resultNum; j++)
                    {
                        CombineDic[tuple].Add(Convert.ToInt32(wordList[3 + j]));
                    }

                    break;
                case FILEINFO.ITEMTABLE:
                    index = Convert.ToInt32(wordList[0]);
                    int sellPrice = Convert.ToInt32(wordList[5]);
                    string group = wordList[2];

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
                        default:
                            break;
                    }

                    FindItemDic[index] = new ItemInfo(index, wordList[1], group, wordList[3], sellPrice, wordList[4], "itemImg/item_" + index);

                    break;
                case FILEINFO.SETITEMTABLE:
                    SetItemInfo setItemInfo = new SetItemInfo(Convert.ToInt32(wordList[0]), Convert.ToInt32(wordList[1]), Convert.ToInt32(wordList[2]), Convert.ToInt32(wordList[3]), Convert.ToInt32(wordList[4]));

                    SetComineList.Add(setItemInfo);

                    break;
                case FILEINFO.QUESTTABLE:
                    index = Convert.ToInt32(wordList[0]);
                    int termsItem = Convert.ToInt32(wordList[4]);
                    int termsCount = Convert.ToInt32(wordList[5]);
                    int reward = Convert.ToInt32(wordList[6]);
                    int rewardCount = Convert.ToInt32(wordList[7]);

                    FindQuestDic[index] = new QuestInfo(index, wordList[1], wordList[2], wordList[3], termsItem, termsCount, reward, rewardCount);

                   
                    if(wordList[1]!=formerSceneName)//씬의 첫 퀘스트의 인덱스 구하기
                    {
                        FirstQuestsOfScene.Add(index);
                        formerSceneName = wordList[1];
                    }

                    break;
                case FILEINFO.UPGRADETABLE:
                    index = Convert.ToInt32(wordList[0]);

                    UpgradeInfo upInfo;
                    upInfo.index = index;
                    upInfo.name = wordList[1];

                    upInfo.value = new int[20];
                    upInfo.cost = new int[20];

                    for (int j = 0; j < 20; j++)
                    {
                        if (wordList.Length < 20) // 에러 회피용 임시코드. 50012번이 특수 아이템이라 wordList.Length가 2밖에 안됩니다.
                        {
                            break;
                        }

                        int value = Convert.ToInt32(wordList[2 * j + 2]);
                        int cost = Convert.ToInt32(wordList[2 * j + 3]);
                        upInfo.value[j] = value;
                        upInfo.cost[j] = cost;
                    }

                    FindUpDic[index] = upInfo;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 기준표에서 아이템을 찾는 함수
    /// </summary>
    /// <param name="key">findDic의 key값</param>
    /// <returns>리턴값</returns>
    public ItemInfo FindItem(int key)
    {
        return FindItemDic[key];
    }

    /// <summary>
    /// 조합표에서 검색하는 함수
    /// </summary>
    /// <param name="key1">재료1의 인덱스</param>
    /// <param name="key2">재료2의 인덱스</param>
    /// <returns>찾았다면 결과를, 아니라면 0을 반환</returns>
    public List<int> FindCombine(int key1, int key2)
    {
        Tuple<int, int> tuple = new Tuple<int, int>(key1, key2);

        if (CombineDic.ContainsKey(tuple))
        {
            return CombineDic[tuple];
        }

        return null;
    }

    public SetItemInfo CheckSetItemCombine(int key)
    {
        DataController dataController = DataController.Instance;
        Dictionary<int, Dictionary<int, SerializableVector3>> haveDic = dataController.HaveDic;

        foreach (SetItemInfo setItemInfo in SetComineList)
        {
            if (key == setItemInfo.index1)
            {
                if (haveDic.ContainsKey(setItemInfo.index2) && haveDic[setItemInfo.index2].Count > 0 && haveDic.ContainsKey(setItemInfo.index3) && haveDic[setItemInfo.index3].Count > 0 && haveDic.ContainsKey(setItemInfo.index4) && haveDic[setItemInfo.index4].Count > 0)
                {
                    return setItemInfo;
                }
            }
            else if (key == setItemInfo.index2)
            {
                if (haveDic.ContainsKey(setItemInfo.index1) && haveDic[setItemInfo.index1].Count > 0 && haveDic.ContainsKey(setItemInfo.index3) && haveDic[setItemInfo.index3].Count > 0 && haveDic.ContainsKey(setItemInfo.index4) && haveDic[setItemInfo.index4].Count > 0)
                {
                    return setItemInfo;
                }
            }
            else if (key == setItemInfo.index3)
            {
                if (haveDic.ContainsKey(setItemInfo.index1) && haveDic[setItemInfo.index1].Count > 0 && haveDic.ContainsKey(setItemInfo.index2) && haveDic[setItemInfo.index2].Count > 0 && haveDic.ContainsKey(setItemInfo.index4) && haveDic[setItemInfo.index4].Count > 0)
                {
                    return setItemInfo;
                }
            }
            else if (key == setItemInfo.index4)
            {
                if (haveDic.ContainsKey(setItemInfo.index1) && haveDic[setItemInfo.index1].Count > 0 && haveDic.ContainsKey(setItemInfo.index2) && haveDic[setItemInfo.index2].Count > 0 && haveDic.ContainsKey(setItemInfo.index3) && haveDic[setItemInfo.index3].Count > 0)
                {
                    return setItemInfo;
                }
            }
        }

        return new SetItemInfo(0, 0, 0, 0, 0);
    }

    /// <summary>
    /// 기준표에서 퀘스트를 찾는 함수
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
}