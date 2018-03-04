﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Since unity doesn't flag the Vector3 as serializable, we need to create our own version. This one will automatically convert between Vector3 and SerializableVector3
/// </summary>
[Serializable]
public struct SerializableVector3
{
    /// <summary>
    /// x component
    /// </summary>
    public float x;

    /// <summary>
    /// y component
    /// </summary>
    public float y;

    /// <summary>
    /// z component
    /// </summary>
    public float z;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}

//업그레이드 변수들 모아놓기
public class UpgradeClass
{
    //업그레이드 관련 변수. 참조 쉽게 하려고 Array로.
    public delegate void UpgradeMethod();
    public UpgradeMethod[] UpgradeMethods;
    public int[] MaxLV=new int[12];

    public const int FIRST_UPGRADE_INDEX= 50001;

    private int[] UpgradeLV = new int[12];

    /// <summary>
    /// id를 통해 현재 업그레이드 레벨을 반환합니다.
    /// </summary>
    /// <param name="id">양자리(0)~물고기자리(11)</param>
    /// <returns></returns>
    public int this[int id]
    {
        get
        {
            if (-1 < id && id < 12)
            {
                return UpgradeLV[id];
            }
            else if (50000<id&&id<=50012)
            {
                return UpgradeLV[id - 50001];
            }
            else
            {
                Debug.LogError("Out of Index; Index should between 0~11 or 50001~50012");
                return 0;
            }
        }
        set
        {
            if (value >= 0)
            {
                
                if (-1 < id && id < 12)
                {
                    UpgradeLV[id]=value;
                    PlayerPrefs.SetInt("Upgrade[" + id + "]LV", value);
                }
                else if (50000 < id && id <= 50012)
                {
                    UpgradeLV[id - 50001]=value;
                    PlayerPrefs.SetInt("Upgrade[" + (id-50001) + "]LV", value);
                }
                else
                {
                    Debug.LogError("Out of Index; Index should between 0~11 or 50001~50012");
                }
                
            }
            else
            {
                Debug.LogWarning("Level Can not be minus.");
            }
        }
    }
    /// <summary>
    /// 업그래이드 인덱스를 통해 현재 레벨을 반환합니다.
    /// </summary>
    /// <param name="index">업그레이드 인덱스(50001~50012)</param>
    /// <returns></returns>
    public int GetLVByUpgradeIndex(int index)
    {
        return this[index - FIRST_UPGRADE_INDEX];
    }

   public UpgradeClass()
    {
        /*for (int i = 0; i < 12; i++)
        {
            this.UpgradeMethods[i] = new UpgradeMethod(() => LevelUp(i));
        }*/
        this.UpgradeLV = new int[12];
    }

    public UpgradeClass(int invenLV, int enegyPerClickLV, int waitTime1LV, int item1LVint, int saleGoldLV, int waitTime2LV, int item2LV, int atlasItemLV, int combineItemLV, int waitTime3LV, int item3LV, int twiceAll)
    {
        this.UpgradeLV = new int[12] { invenLV, enegyPerClickLV, waitTime1LV, item1LVint, saleGoldLV, waitTime2LV, item2LV, atlasItemLV, combineItemLV, waitTime3LV, item3LV, twiceAll };
    }

    public void LevelUp(int id)
    {
        this.UpgradeLV[id] ++;
        PlayerPrefs.SetInt("Upgrade[" + id + "]LV", this.UpgradeLV[id]);
    }
}

public class DataController : MonoBehaviour
{
    // 현재 보유 골드량
    private ulong m_gold;

    // 로딩 유무
    private bool loadingFinish;

    // 현재 보유 아이템 개수, 퀘스트 진행도(인덱스)
    private int m_itemcount, m_questProcess, m_energy, m_latestUpgradeIndex;

    // 아이템 타이머 시간
    private float[] m_leftTimer;

    // HaveDic 정보 저장 경로
    public string HaveDicPath { get; private set; }

    // itemOpenList 정보 저장 경로
    public string ItemOpenListPath { get; private set; }

    /// <summary>
    /// (임시)이벤트 관찰자 목록
    /// </summary>
    public List<GameObject> Observers;

    // 튜토리얼 완료 여부
    public int isTutorialEnd;

    public bool IsTutorialEnd
    {
        get
        {
            return isTutorialEnd>0;
        }
        private set
        {
            isTutorialEnd = value?1:0;
            PlayerPrefs.SetInt("TutorialEnd", isTutorialEnd);
        }
    }

    // 튜토리얼 현재 인덱스
    private int nowIndex;

    public int NowIndex
    {
        get
        {
            switch (nowIndex)
            {
                case 300136:
                    nowIndex = 300201;
                    break;
                case 300218:
                    nowIndex = 300301;
                    break;
                case 300311:
                    nowIndex = 300401;
                    break;
                case 300428:
                    nowIndex = 300501;
                    break;
                case 300516:
                    nowIndex = 300601;
                    break;
                case 300627:
                    IsTutorialEnd = true;
                    nowIndex = 300701;
                    return 0;
            }

            return nowIndex;
        }
        set
        {
            nowIndex = value;
            PlayerPrefs.SetInt("NowIndex", nowIndex);
        }
    }

    private DataDictionary dataDic;

    /// <summary>
    /// NOTE: 현재 내가 소지하고 있는 재료 Dictionary
    /// <para>-> key(int) : 게임 오브젝트를 구별하는 id</para>
    /// <para>-> value(Dictionary<int, SerializableVector3>) : 인스턴스 ID, 위치</para>
    /// </summary>
    public Dictionary<int, Dictionary<int, SerializableVector3>> HaveDic { get; private set; }

    /// <summary>
    /// NOTE: 열린 도감을 저장하는 Dictionary
    /// <para>-> key(int) : 도감이 열린 재료 Index</para>
    /// </summary>
    [HideInInspector]
    public List<int> itemOpenList;

    //신규 항목알림을 위한 더티 플래그. 서적과 아이템 리스트는 새로운 아이템들의 인덱스를 담음. 길이 0이면 신규 없음.
    [HideInInspector]
    //public bool newQuest; 나중에 사용 안 하면 지울 것.
   
    public List<int> newBookList;
    public List<int> newItemList;
    public string NewBookListPath { get; private set; }
    public string NewItemListPath { get; private set; }

    [HideInInspector]
    private int newUpgradeInt;
    public bool NewUpgrade
    {
        get
        {
            return newUpgradeInt > 0;
        }
        set
        {
            if (value)
            {
                newUpgradeInt = 1;
            }
            else
            {
                newUpgradeInt = 0;
            }
            PlayerPrefs.SetInt("NewUpgrade", newUpgradeInt);
        }
    }
    public static UpgradeClass upgradeLV;

    #region Singleton

    private static DataController instance;

    public static DataController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataController>();

                if (instance == null)
                {
                    GameObject container = new GameObject("DataController");
                    instance = container.AddComponent<DataController>();
                }
            }

            return instance;
        }
    } 
    #endregion

    // 게임 초기화될 때 
    public void Awake()
    {
        DontDestroyOnLoad(this);

        dataDic = DataDictionary.Instance;

        loadingFinish = false;

        // Key : Value로써 PlayerPrefs에 저장
        m_gold = Convert.ToUInt64(PlayerPrefs.GetString("Gold", "0"));
        m_itemcount = PlayerPrefs.GetInt("ItemCount", 0);
        m_questProcess = PlayerPrefs.GetInt("QuestProcess", 90101);
        m_leftTimer = new float[3] { PlayerPrefs.GetFloat("LeftTimer1", 300.0f), PlayerPrefs.GetFloat("LeftTimer2", 300.0f), PlayerPrefs.GetFloat("LeftTimer3", 300.0f) };
        m_energy = PlayerPrefs.GetInt("Energy", 0);
        m_latestUpgradeIndex = PlayerPrefs.GetInt("LatestUpgrade", 50000);

        //업그레이드 레벨 변수들을 하나로 합침
        upgradeLV = new UpgradeClass();

        for(int i=0; i<12; i++)
        {
            upgradeLV[i]= PlayerPrefs.GetInt("Upgrade[" + i + "]LV", 0);
        }



        newUpgradeInt = PlayerPrefs.GetInt("NewUpgrade",0);

        isTutorialEnd = PlayerPrefs.GetInt("TutorialEnd", 0);

        // 튜토리얼 인덱스를 파트로 구분하여, 파트 중간에 종료 시 파트 처음으로 돌아가게 처리함.
        NowIndex = PlayerPrefs.GetInt("NowIndex", 300101) % 100000 / 100 * 100 + 300001;

        HaveDicPath = "/haveDic.txt";
        ItemOpenListPath = "/itemOpenList.txt";
        NewBookListPath = "/newBookList.txt";
        NewItemListPath = "/newItemList.txt";
        
        
        HaveDic = LoadGameData(HaveDicPath) as Dictionary<int, Dictionary<int, SerializableVector3>>;

        if (HaveDic == null)
        {
            HaveDic = new Dictionary<int, Dictionary<int, SerializableVector3>>();
        }

        itemOpenList = LoadGameData(ItemOpenListPath) as List<int>;

        if (itemOpenList == null)
        {
            itemOpenList = new List<int>();
        }

        newBookList = LoadGameData(NewBookListPath) as List<int>;

        if (newBookList == null)
        {
            newBookList = new List<int>();
        }

        newItemList = LoadGameData(NewItemListPath) as List<int>;

        if (newItemList == null)
        {
            newItemList = new List<int>();
        }
        

        
    }

    void Start()
    {
        // 종료 후 실행 시간 계산
        for (int i = 0; i < 3; i++)
        {
            m_leftTimer[i] -= TimeAfterLastPlay;
        }

        InvokeRepeating("OnApplicationQuit", 0f, 5f);

        LoadingFinish = true;
    }

    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            m_leftTimer[i] -= Time.deltaTime;
            PlayerPrefs.SetFloat("LeftTimer" + (i + 1), m_leftTimer[i]);
        }
    }

    // 게임 데이터를 불러오는 함수
    public object LoadGameData(string dataPath)
    {
        string filePath = Application.persistentDataPath + dataPath;

        if (File.Exists(filePath))
        {
            return DataDeserialize(File.ReadAllBytes(filePath));
        }

        return null;
    }

    // 데이터를 역직렬화하는 함수
    public object DataDeserialize(byte[] buffer)
    {
        BinaryFormatter binFormatter = new BinaryFormatter();
        MemoryStream mStream = new MemoryStream();

        mStream.Write(buffer, 0, buffer.Length);
        mStream.Position = 0;

        return binFormatter.Deserialize(mStream);
    }

    // 게임 데이터를 저장하는 함수
    public void SaveGameData(object data, string dataPath)
    {
        byte[] stream = DataSerialize(data);
        File.WriteAllBytes(Application.persistentDataPath + dataPath, stream);
    }

    // 데이터를 직렬화하는 함수
    public byte[] DataSerialize(object data)
    {
        BinaryFormatter binFormmater = new BinaryFormatter();
        MemoryStream mStream = new MemoryStream();

        binFormmater.Serialize(mStream, data);

        return mStream.ToArray();
    }

    // 플레이 종료 시간 가져오기
    DateTime GetLastPlayDate()
    {
        if (!PlayerPrefs.HasKey("Time"))
        {
            return DateTime.Now;
        }

        return DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("Time")));
    }

    // 어플 종료 시 플레이 시간 저장
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("Time", DateTime.Now.ToBinary().ToString());
    }

    // 종료 후 지난 시간 계산
    public int TimeAfterLastPlay
    {
        get
        {
            DateTime currentTime = DateTime.Now;
            DateTime lastTime = GetLastPlayDate();

            int subTime = (int)currentTime.Subtract(lastTime).TotalSeconds;
            return subTime;
        }
    }

    // 로딩 상태 가져오기
    public bool LoadingFinish
    {
        get
        {
            return loadingFinish;
        }
        set
        {
            loadingFinish = value;
        }
    }

    /// <summary>
    /// gold 설정
    /// </summary>
    public ulong Gold
    {
        get
        {
            return m_gold;
        }
        set
        {
            ulong delta = value - m_gold;
            m_gold = value;
            PlayerPrefs.SetString("Gold", m_gold.ToString());

            foreach (GameObject target in Observers)//관찰자들에게 이벤트 메세지 송출
            {
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnChangeValue(ValueType.Gold, value, delta));
            }

        }
    }

    // item 개수 세는 함수
    public int ItemCount
    {
        get
        {
            return m_itemcount;
        }

        set
        {
            if (value < 0)
            {
                m_itemcount = 0;
            }
            else
            {
                m_itemcount = value;
            }

            PlayerPrefs.SetInt("ItemCount", m_itemcount);
        }
    }

    // item 최대 개수 가져오기 
    public int ItemLimit
    {
        get
        {
            if (upgradeLV[0] == 0)
            {
                return 10;
            }
            else
            {
                return 10 + dataDic.FindUpgrade(50001).value[upgradeLV[0] - 1];
            }
        }
    }

  
    // 인벤토리 레벨 업그레이드
    public void UpgradeInvenLv()
    {
        upgradeLV[0] += 1;
        PlayerPrefs.SetInt("InvenLevel", upgradeLV[0]);
    }

    // 에너지량 가져오기
    public int Energy
    {
        get
        {
            return m_energy;
        }
        set
        {
            m_energy = value;
            PlayerPrefs.SetInt("Energy", m_energy);
        }
    }

    /// <summary>
    /// EnergyPerClick을 얻는 함수
    /// </summary>
    /// <returns></returns>
    public int EnergyPerClick
    {
        get
        {
            if (upgradeLV[1] == 0)
            {
                return 2;
            }
            else
            {
                return 2 + dataDic.FindUpgrade(50002).value[upgradeLV[1] - 1];
            }
        }
    }
    /// <summary>
    /// 클릭 당 게이지 레벨 업그레이드
    /// </summary>
    public void UpgradeEnergyPerClickLv()
    {
        upgradeLV[1] += 1;

    }

    /// <summary>
    /// 지구본에서 일반 아이템이 나올 확률(0~95)
    /// </summary>
    public int AtlasItemProb
    {
        get
        {
            return 95 - upgradeLV[7];
        }
    }

    #region Item
    public void InsertNewItem(int key, int itemId)
    {
        InsertNewItem(key, itemId, new Vector3(0, 0, 0));
    }

    /// <summary>
    /// 아이템을 추가하는 함수
    /// </summary>
    /// <param name="key">추가하는 아이템의 Index</param>
    public void InsertNewItem(int key, int itemId, Vector3 itemPos)
    {
        if (!CheckExistItem(key))
        {
            itemOpenList.Add(key);
            SaveGameData(itemOpenList, ItemOpenListPath);

            //Debug.Log("itemOpenList - DataSerialize");

            Dictionary<int, SerializableVector3> posList = new Dictionary<int, SerializableVector3>
            {
                { itemId, itemPos }
            };

            HaveDic.Add(key, posList);

            //!마크 띄우기
            if (key > 4000)//서적
            {
                newBookList.Add(key);
                SaveGameData(newBookList, NewBookListPath);
                SetItemInfo setInfo = dataDic.CheckSetItemCombine(key);
                if (setInfo.result != 0)
                {
                    InsertNewItem(setInfo.result, 1);
                }
            }
            else//도감
            {
                newItemList.Add(key);
                SaveGameData(newItemList, NewItemListPath);
            }
        }
        else
        {
            int id=itemId;
            if (HaveDic[key].ContainsKey(id))//id 에러 방지용 땜빵 코드. itemTimer 부분 참조.
            {
                while (HaveDic[key].ContainsKey(id))
                {
                    id++;
                }

                
            }
            HaveDic[key].Add(id, itemPos);
        }

        foreach (GameObject target in Observers)//관찰자들에게 이벤트 메세지 송출
        {
            ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObtain(dataDic.FindItemDic[key]));
        }

        SaveGameData(HaveDic, HaveDicPath);

        //Debug.Log("InsertNewItem - HaveDic DataSerialize");
    }

    // 퀘스트에 쓰이는 아이템 추가 함수
    // 퀘스트 쪽 로직을 아직 잘 몰라 일단 돌아가게끔 추가했고
    // 필요 없으면 삭제 등의 조치를 취할 것.
    //public void InsertNewItem(int key, int itemNum)
    //{
    //    for (int i = 0; i < itemNum; i++)
    //    {
    //        InsertNewItem(key, new Vector3(UnityEngine.Random.Range(175, 953), UnityEngine.Random.Range(616, -227), -3));
    //    }
    //}

    /// <summary>
    /// 아이템을 삭제하는 함수
    /// </summary>
    /// <param name="key">삭제할 아이템의 key값</param>
    public void DeleteItem(int key, int itemId)
    {
        if (!HaveDic[key].Remove(itemId))
        {
            Debug.Log("DataController - DeleteItem : Item Cannot Delete.");
        }

        SaveGameData(HaveDic, HaveDicPath);
    }

    /// <summary>
    /// 현재 이 아이템을 보유하고 있는지 확인하는 함수
    /// </summary>
    /// <param name="key">haveDic의 key값</param>
    /// <returns></returns>
    public bool CheckExistItem(int key)
    {
        return HaveDic.ContainsKey(key);
    }

    /// <summary>
    /// 현재 보유하고 있는 아이템의 갯수를 보여주는 함수
    /// </summary>
    /// <param name="key">haveDic의 key값</param>
    /// <returns></returns>
    public int GetItemNum(int key)
    {
        if (CheckExistItem(key))
        {
            return HaveDic[key].Count;
        }

        return 0;
    } 
    #endregion

    #region Quest

    public int QuestProcess
    {
        get
        {
            return m_questProcess;
        }
    }

    /// <summary>
    /// 다음 퀘스트로 넘어가기
    /// </summary>
    public void NextQuest()
    {
        m_questProcess += 1;
        PlayerPrefs.SetInt("QuestProcess", m_questProcess);
    }
    #endregion

    #region Item Timer
    /// <summary>
    /// 인덱서. id를 통해 타이머 남은 시간 반환.
    /// </summary>
    /// <param name="index">ItemTimer의 id</param>
    /// <returns>t남은 시간</returns>
    public float this[int index]
    {
        get
        {
            return m_leftTimer[index];
        }

        set
        {
            m_leftTimer[index] = value;
            PlayerPrefs.SetFloat("LeftTimer"+(index+1), m_leftTimer[index]);
        }
    } 
    #endregion

    /// <summary>
    /// 열려있는 업그레이드 중 가장 큰 인덱스를 반환합니다. (50001~50012)
    /// </summary>
    public int LatestUpgradeIndex
    {
        get
        {
            return m_latestUpgradeIndex;
        }
        set
        {
            if (value >= 50000)
            {
                if (value-m_latestUpgradeIndex>1)
                {
                    Debug.LogWarning("순차적인 업그레이드가 아닙니다.");
                }
                m_latestUpgradeIndex = value;
                PlayerPrefs.SetInt("LatestUpgrade", value);
            }
            else
            {
                Debug.LogWarning("Upgrade index can not be lower than 50000");
            }
        }
    }

    // 업그레이드 인덱스로 최대 업그레이드 레벨 올리기
    public void UnlockUpgrade(int index)
    {

        upgradeLV.MaxLV[index-50001] = dataDic.FindUpgrade(index).value.Length;//값길이=최대 길이
        
        NewUpgrade = true;
    }

    public void ResetForDebug()
    {
        PlayerPrefs.DeleteAll();


        HaveDic.Clear();
        itemOpenList.Clear();
        newBookList.Clear();
        newItemList.Clear();




        SaveGameData(HaveDic, HaveDicPath);
        SaveGameData(itemOpenList, ItemOpenListPath);
        SaveGameData(newBookList, NewBookListPath);
        SaveGameData(newItemList, NewItemListPath);

        m_gold = Convert.ToUInt64(PlayerPrefs.GetString("Gold", "0"));
        m_itemcount = PlayerPrefs.GetInt("ItemCount", 0);
        m_questProcess = PlayerPrefs.GetInt("QuestProcess", 90101);
        m_leftTimer = new float[3] { PlayerPrefs.GetFloat("LeftTimer1", 0f), PlayerPrefs.GetFloat("LeftTimer2", 0f), PlayerPrefs.GetFloat("LeftTimer3", 0f) };//리셋 즉시 시험할 수 있게
        m_energy = PlayerPrefs.GetInt("Energy", 0);
        m_latestUpgradeIndex = PlayerPrefs.GetInt("LatestUpgrade", 50000);


        upgradeLV = new UpgradeClass();

        for (int i = 0; i < 12; i++)
        {
            upgradeLV[i] = PlayerPrefs.GetInt("Upgrade[" + i + "]LV", 0);
        }
    }
}