using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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

    // HaveDic 정보 저장 경로
    public string HaveDicPath { get; private set; }

    // itemOpenList 정보 저장 경로
    public string ItemOpenListPath { get; private set; }

    // 튜토리얼 완료 여부
    public int isTutorialEnd;

    public int IsTutorialEnd
    {
        get
        {
            return isTutorialEnd;
        }
        private set
        {
            isTutorialEnd = value;
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
                    IsTutorialEnd = 1;
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
    //public bool newUpgrade;
    public List<int> newBookList;
    public List<int> newItemList;
    public string NewBookListPath { get; private set; }
    public string NewItemListPath { get; private set; }

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

        invenLv = PlayerPrefs.GetInt("InvenLevel", 0);
        energyPerClickLv = PlayerPrefs.GetInt("EnergyPerClickLevel", 0);

        invenMaxLv = PlayerPrefs.GetInt("InvenMaxLevel", 0);
        energyPerClickMaxLv = PlayerPrefs.GetInt("EnergyPerClickMaxLevel", 0);

        IsTutorialEnd = PlayerPrefs.GetInt("TutorialEnd", 0);

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
            m_gold = value;
            PlayerPrefs.SetString("Gold", m_gold.ToString());
            
            if (IsTutorialEnd == 0 && NowIndex == 300427 && m_gold >= 200)
            {
                GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>().ContinueDialogue();
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
            HaveDic[key].Add(itemId, itemPos);
        }

        if (IsTutorialEnd == 0 && NowIndex == 300310)
        {
            int sum = 0;

            foreach (KeyValuePair<int, Dictionary<int, SerializableVector3>> entry in HaveDic)
            {
                switch(entry.Key)
                {
                    case 1004: case 1005: case 1006:
                        sum += entry.Value.Count;
                        break;
                }
            }

            if (sum >= 2)
            {
                GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>().ContinueDialogue();
            }
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
        m_leftTimer = new float[3] { PlayerPrefs.GetFloat("LeftTimer1", 300.0f), PlayerPrefs.GetFloat("LeftTimer2", 300.0f), PlayerPrefs.GetFloat("LeftTimer3", 300.0f) };
        m_energy = PlayerPrefs.GetInt("Energy", 0);

        invenLv = PlayerPrefs.GetInt("InvenLevel", 0);
        energyPerClickLv = PlayerPrefs.GetInt("EnergyPerClickLevel", 0);

        invenMaxLv = PlayerPrefs.GetInt("InvenMaxLevel", 0);
        energyPerClickMaxLv = PlayerPrefs.GetInt("EnergyPerClickMaxLevel", 0);

        for (int i = 0; i < 3; i++)
        {
            this[i] = 0;
        }
    }
}