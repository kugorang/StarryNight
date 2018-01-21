using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataController : MonoBehaviour
{
    // 현재 보유 골드량
    private ulong m_gold;

    // 로딩 유무
    private bool loadingFinish;

    // 현재 보유 아이템 개수, 퀘스트 진행도(인덱스)
    private int m_itemcount, m_questProcess, m_energy;

    // 아이템 타이머 시간
    private float[] m_leftTimer;

    // 현재 인벤토리 레벨, 클릭 게이지 레벨
    private int invenLv, energyPerClickLv;

    // 최대 업그레이드 가능한 - 인벤토리 레벨, 클릭 게이지 레벨
    private int invenMaxLv, energyPerClickMaxLv;

    // haveDic 정보 저장 경로
    public string HaveDicPath { get; private set; }

    // itemOpenList 정보 저장 경로
    public string ItemOpenListPath { get; private set; }

    private DataDictionary dataDic;

    /// <summary>
    /// NOTE: 현재 내가 소지하고 있는 재료 Dictionary
    /// <para>-> key(int) : 게임 오브젝트를 구별하는 id</para>
    /// <para>-> value(HaveDicInfo) : 재료 기준표 정보</para>
    /// </summary>
    public Dictionary<int, int> haveDic;

    /// <summary>
    /// NOTE: 열린 도감을 저장하는 Dictionary
    /// <para>-> key(int) : 도감이 열린 재료 index</para>
    /// </summary>
    [HideInInspector]
    public List<int> itemOpenList;

    private static DataController instance;

    public static DataController GetInstance()
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

    // 게임 초기화될 때 
    void Awake()
    {
        DontDestroyOnLoad(this);

        dataDic = GameObject.FindWithTag("DataController").GetComponent<DataDictionary>();

        loadingFinish = false;

        // Key : Value로써 PlayerPrefs에 저장
        m_gold = Convert.ToUInt64(PlayerPrefs.GetString("Gold", "0"));
        m_itemcount = PlayerPrefs.GetInt("ItemCount", 0);
        m_questProcess = PlayerPrefs.GetInt("QuestProcess", 90101);
        m_leftTimer = new float[3] { PlayerPrefs.GetFloat("LeftTimer1", 300.0f), PlayerPrefs.GetFloat("LeftTimer2", 300.0f), PlayerPrefs.GetFloat("LeftTimer3", 300.0f) };
        m_energy = PlayerPrefs.GetInt("Energy", 0);

        invenLv = PlayerPrefs.GetInt("InvenLevel", 0);
        energyPerClickLv = PlayerPrefs.GetInt("EnergyPerClickLevel", 0);

        invenMaxLv = PlayerPrefs.GetInt("InvenMaxLevel", 0);
        energyPerClickMaxLv = PlayerPrefs.GetInt("EnergyPerClickMaxLevel", 0);

        HaveDicPath = "/haveDic.txt";
        ItemOpenListPath = "/itemOpenList.txt";

        haveDic = LoadGameData(HaveDicPath) as Dictionary<int, int>;

        if (haveDic == null)
        {
            haveDic = new Dictionary<int, int>();
        }

        itemOpenList = LoadGameData(ItemOpenListPath) as List<int>;

        if (itemOpenList == null)
        {
            itemOpenList = new List<int>();
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
            m_gold = value;
            PlayerPrefs.SetString("Gold", m_gold.ToString());
        }
    }
    // item 개수 세는 함수
    public int ItemCount
    {
        get
        {
            return m_itemcount;
        }
    }

    // item 개수 더하는 함수
    public void AddItemCount()
    {
        m_itemcount += 1;
        PlayerPrefs.SetInt("ItemCount", m_itemcount);
    }

    public void SubItemCount()
    {
        m_itemcount -= 1;
        PlayerPrefs.SetInt("ItemCount", m_itemcount);
    }

    // item 최대 개수 가져오기 
    public int ItemLimit
    {
        get
        {
            if (invenLv == 0)
            {
                return 10;
            }
            else
            {
                return 10 + dataDic.FindUpgrade(50001).value[invenLv - 1];
            }
        }
    }

    // 인벤토리 레벨 가져오기
    public int InvenLv
    {
        get
        {
            return invenLv;
        }
    }

    // 인벤토리 레벨 업그레이드
    public void UpgradeInvenLv()
    {
        invenLv += 1;
        PlayerPrefs.SetInt("InvenLevel", invenLv);
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
            return 2 + 2 * energyPerClickLv;
        }
    }
    /// <summary>
    /// 클릭 당 게이지 레벨 가져오기
    /// </summary>
    /// <returns></returns>
    public int EnergyPerClickLv
    {
        get
        {
            return energyPerClickLv;
        }
    }

    /// <summary>
    /// 클릭 당 게이지 레벨 업그레이드
    /// </summary>
    public void UpgradeEnergyPerClickLv()
    {
        energyPerClickLv += 1;
        PlayerPrefs.SetInt("EnergyPerClickLevel", energyPerClickLv);
    }

    /// <summary>
    /// 아이템을 추가하는 함수
    /// </summary>
    /// <param name="key">추가하는 아이템의 index</param>
    public void InsertItem(int key, int addNum)
    {
        if (!CheckExistItem(key))
        {
            itemOpenList.Add(key);
            SaveGameData(itemOpenList, ItemOpenListPath);

            //Debug.Log("itemOpenList - DataSerialize");

            haveDic.Add(key, 1);
        }
        else
        {
            haveDic[key] += addNum;
        }

        SaveGameData(haveDic, HaveDicPath);

        //Debug.Log("InsertItem - haveDic DataSerialize");
    }

    /// <summary>
    /// 아이템을 삭제하는 함수
    /// </summary>
    /// <param name="key">삭제할 아이템의 key값</param>
    public void DeleteItem(int key)
    {
        if (haveDic[key] < 0)
        {
            // 디버깅용 코드
            Debug.LogError("Delete Item is under 0");
        }

        haveDic[key]--;

        //UM_Storage.Save("haveDic", DataSerialize(haveDic));
        SaveGameData(haveDic, HaveDicPath);

        //Debug.Log("DeleteItem() - haveDic DataSerialize");
    }

    /// <summary>
    /// 현재 이 아이템을 보유하고 있는지 확인하는 함수
    /// </summary>
    /// <param name="key">haveDic의 key값</param>
    /// <returns></returns>
    public bool CheckExistItem(int key)
    {
        return haveDic.ContainsKey(key);
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
            return haveDic[key];
        }

        return 0;
    }

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


    public float this[int index]
    {
        get
        {
            return m_leftTimer[index];
        }

        set
        {
            m_leftTimer[index] = value;
            PlayerPrefs.SetFloat("LeftTimer1", m_leftTimer[index]);
        }
    }


    /// <summary>
    /// 최대 업그레이드 가능한 인벤토리 레벨
    /// </summary>

    public int MaxInvenLv
    {
        get
        {
            return invenMaxLv;
        }

        set
        {
            invenMaxLv = value;
            PlayerPrefs.SetInt("InvenMaxLevel", invenMaxLv);
        }
    }

    /// <summary>
    /// 최대 업그레이드 가능한 클릭당 에너지 레벨 가져오기
    /// </summary>
    /// <returns></returns>

    /// <summary>
    /// 최대 업그레이드 가능한 클릭당 에너지 레벨 설정
    /// </summary>
    /// <param name="num">최대 레벨</param>
    public int MaxPerClickLv
    {
        get
        {
            return energyPerClickMaxLv;
        }

        set
        {
            energyPerClickMaxLv = value;
            PlayerPrefs.SetInt("EnergyPerClickMaxLevel", energyPerClickMaxLv);
        }
    }

    // 업그레이드 인덱스로 현재 업그레이드 레벨 찾기
    public int CheckUpgradeLevel(int index)
    {
        if (index == 50001)
        {
            return invenLv;
        }
        else if (index == 50002)
        {
            return energyPerClickLv;
        }
        else
        {
            return 0;
        }
    }

    // 업그레이드 인덱스로 최대 업그레이드 레벨 올리기
    public void SetMaxUpgradeLevel(int index)
    {
        if (index == 50001)
        {
            MaxInvenLv = 20;
        }
        else if (index == 50002)
        {
            MaxPerClickLv = 20;
        }
    }
}