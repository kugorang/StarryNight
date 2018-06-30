using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script
{
    /// <summary>
    ///     Since unity doesn't flag the Vector3 as serializable, we need to create our own version. This one will
    ///     automatically convert between Vector3 and SerializableVector3
    /// </summary>
    [Serializable]
    public struct SerializableVector3
    {
        /// <summary>
        ///     x component
        /// </summary>
        private readonly float _x;

        /// <summary>
        ///     y component
        /// </summary>
        private readonly float _y;

        /// <summary>
        ///     z component
        /// </summary>
        private readonly float _z;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        /// <param name="rZ"></param>
        private SerializableVector3(float rX, float rY, float rZ)
        {
            _x = rX;
            _y = rY;
            _z = rZ;
        }

        /// <summary>
        ///     Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", _x, _y, _z);
        }

        /// <summary>
        ///     Automatic conversion from SerializableVector3 to Vector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Vector3(SerializableVector3 rValue)
        {
            return new Vector3(rValue._x, rValue._y, rValue._z);
        }

        /// <summary>
        ///     Automatic conversion from Vector3 to SerializableVector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator SerializableVector3(Vector3 rValue)
        {
            return new SerializableVector3(rValue.x, rValue.y, rValue.z);
        }
    }

    // 업그레이드 변수들 모아놓기
    public class UpgradeClass
    {
        // 업그레이드 관련 변수. 참조 쉽게 하려고 Array로.
        public delegate void UpgradeMethod();
        private const int FirstUpgradeIndex = 50001;
        public readonly int[] MaxLv = new int[12];

        private readonly int[] _upgradeLv;
        public UpgradeMethod[] UpgradeMethods;

        public UpgradeClass()
        {
            /*for (int i = 0; i < 12; i++)
        {
            this.UpgradeMethods[i] = new UpgradeMethod(() => LevelUp(i));
        }*/
            _upgradeLv = new int[12];
        }

        public UpgradeClass(int invenLv, int enegyPerClickLv, int waitTime1Lv, int item1LVint, int saleGoldLv, 
            int waitTime2Lv, int item2Lv, int atlasItemLv, int combineItemLv, int waitTime3Lv, int item3Lv, int twiceAll)
        {
            _upgradeLv = new int[12]
            {
                invenLv, enegyPerClickLv, waitTime1Lv, item1LVint, saleGoldLv, waitTime2Lv, item2Lv, atlasItemLv,
                combineItemLv, waitTime3Lv, item3Lv, twiceAll
            };
        }

        /// <summary>
        ///     id를 통해 현재 업그레이드 레벨을 반환합니다.
        /// </summary>
        /// <param name="id">양자리(0)~물고기자리(11)</param>
        /// <returns></returns>
        public int this[int id]
        {
            get
            {
                if (-1 < id && id < 12) 
                    return _upgradeLv[id];

                if (50000 < id && id <= 50012) 
                    return _upgradeLv[id - 50001];

                Debug.LogError("Out of Index; Index should between 0~11 or 50001~50012");
                return 0;
            }
            set
            {
                if (value >= 0)
                {
                    if (-1 < id && id < 12)
                    {
                        _upgradeLv[id] = value;
                        PlayerPrefs.SetInt("Upgrade[" + id + "]LV", value);
                    }
                    else if (50000 < id && id <= 50012)
                    {
                        _upgradeLv[id - 50001] = value;
                        PlayerPrefs.SetInt("Upgrade[" + (id - 50001) + "]LV", value);
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
        ///     업그레이드 인덱스를 통해 현재 레벨을 반환합니다.
        /// </summary>
        /// <param name="index">업그레이드 인덱스(50001~50012)</param>
        /// <returns></returns>
        public int GetLvByUpgradeIndex(int index)
        {
            return this[index - FirstUpgradeIndex];
        }

        public void LevelUp(int id)
        {
            _upgradeLv[id]++;
            PlayerPrefs.SetInt("Upgrade[" + id + "]LV", _upgradeLv[id]);
        }
    }

    public class DataController : MonoBehaviour
    {
        public static UpgradeClass UpgradeLv;
        private DataDictionary _dataDic;

        // 튜토리얼 완료 여부
        public int TutorialEnd;

        /// <summary>
        ///     NOTE: 열린 도감을 저장하는 Dictionary
        ///     <para>-> key(int) : 도감이 열린 재료 Index</para>
        /// </summary>
        [HideInInspector] 
        public List<int> ItemOpenList;

        // 현재 보유 골드량
        private ulong _gold;

        // 현재 보유 아이템 개수, 퀘스트 진행도(인덱스)
        private int _itemcount, _energy, _latestUpgradeIndex;

        // 아이템 타이머 시간
        private float[] _leftTimer;

        // 신규 항목알림을 위한 더티 플래그. 서적과 아이템 리스트는 새로운 아이템들의 인덱스를 담음. 길이 0이면 신규 없음.
        [HideInInspector]
        // public bool newQuest; 나중에 사용 안 하면 지울 것.
        public List<int> NewBookList;
        public List<int> NewItemList;

        [HideInInspector] 
        private int _newUpgradeInt;

        // 튜토리얼 현재 인덱스
        private int _nowIndex;

        /// <summary>
        ///     (임시) 이벤트 관찰자 목록
        /// </summary>
        public List<GameObject> Observers;

        // HaveDic 정보 저장 경로
        public string HaveDicPath { get; private set; }

        // itemOpenList 정보 저장 경로
        public string ItemOpenListPath { get; private set; }

        public bool IsTutorialEnd
        {
            get
            {
                return TutorialEnd > 0;
            }
            private set
            {
                TutorialEnd = value ? 1 : 0;
                PlayerPrefs.SetInt("TutorialEnd", TutorialEnd);
            }
        }

        public int NowIndex
        {
            get
            {
                if (_nowIndex == 300136)
                {
                    _nowIndex = 300201;
                }
                else if (_nowIndex == 300218)
                {
                    _nowIndex = 300301;
                }
                else if (_nowIndex == 300311)
                {
                    _nowIndex = 300401;
                }
                else if (_nowIndex == 300428)
                {
                    _nowIndex = 300501;
                }
                else if (_nowIndex == 300516)
                {
                    _nowIndex = 300601;
                }
                else if (_nowIndex == 300627)
                {
                    IsTutorialEnd = true;
                    Observers.Remove(DialogueManager.Instance.gameObject);
                    _nowIndex = 300701;
                    return 0;
                }

                return _nowIndex;
            }
            set
            {
                _nowIndex = value;
                PlayerPrefs.SetInt("NowIndex", _nowIndex);
            }
        }

        /// <summary>
        ///     NOTE: 현재 내가 소지하고 있는 재료 Dictionary
        ///     <para>-> key(int) : 게임 오브젝트를 구별하는 id</para>
        ///     <para>-> value(Dictionary<int, SerializableVector3>) : 인스턴스 ID, 위치</para>
        /// </summary>
        public Dictionary<int, Dictionary<int, SerializableVector3>> HaveDic { get; private set; }

        public string NewBookListPath { get; private set; }
        public string NewItemListPath { get; private set; }

        public bool NewUpgrade
        {
            get { return _newUpgradeInt > 0; }
            set
            {
                _newUpgradeInt = value ? 1 : 0;
                PlayerPrefs.SetInt("NewUpgrade", _newUpgradeInt);
            }
        }

        // 종료 후 지난 시간 계산
        private static int TimeAfterLastPlay
        {
            get
            {
                var currentTime = DateTime.Now;
                var lastTime = GetLastPlayDate();

                var subTime = (int) currentTime.Subtract(lastTime).TotalSeconds;
                return subTime;
            }
        }

        // 로딩 상태 가져오기
        public bool LoadingFinish { get; private set; }

        /// <summary>
        ///     gold 설정
        /// </summary>
        public ulong Gold
        {
            get
            {
                return _gold;
            }
            set
            {
                var delta = value - _gold;
                _gold = value;
                PlayerPrefs.SetString("Gold", _gold.ToString());

                // 관찰자들에게 이벤트 메세지 송출
                foreach (var target in Observers) 
                    ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnChangeValue(ValueType.Gold, value, delta));
            }
        }

        // item 개수 세는 함수
        public int ItemCount
        {
            get
            {
                return _itemcount;
            }
            set
            {
                _itemcount = value < 0 ? 0 : value;
                PlayerPrefs.SetInt("ItemCount", _itemcount);
            }
        }

        // item 최대 개수 가져오기 
        public int ItemLimit
        {
            get
            {
                if (UpgradeLv[0] == 0)
                    return 10;
                return 10 + _dataDic.FindUpgrade(50001).value[UpgradeLv[0] - 1];
            }
        }

        // 에너지량 가져오기
        public int Energy
        {
            get { return _energy; }
            set
            {
                _energy = value;
                PlayerPrefs.SetInt("Energy", _energy);
            }
        }

        /// <summary>
        ///     EnergyPerClick을 얻는 함수
        /// </summary>
        /// <returns></returns>
        public int EnergyPerClick
        {
            get
            {
                if (UpgradeLv[1] == 0)
                    return 2;
                return 2 + _dataDic.FindUpgrade(50002).value[UpgradeLv[1] - 1];
            }
        }

        /// <summary>
        ///     지구본에서 일반 아이템이 나올 확률(0~95)
        /// </summary>
        public static int AtlasItemProb
        {
            get { return 95 - UpgradeLv[7]; }
        }

        #region Item Timer

        /// <summary>
        ///     인덱서. id를 통해 타이머 남은 시간 반환.
        /// </summary>
        /// <param name="index">ItemTimer의 id</param>
        /// <returns>t남은 시간</returns>
        public float this[int index]
        {
            get { return _leftTimer[index]; }
            set
            {
                _leftTimer[index] = value;
                PlayerPrefs.SetFloat("LeftTimer" + (index + 1), _leftTimer[index]);
            }
        }

        #endregion

        /// <summary>
        ///     열려있는 업그레이드 중 가장 큰 인덱스를 반환합니다. (50001~50012)
        /// </summary>
        public int LatestUpgradeIndex
        {
            get { return _latestUpgradeIndex; }
            set
            {
                if (value >= 50000)
                {
                    if (value - _latestUpgradeIndex > 1) Debug.LogWarning("순차적인 업그레이드가 아닙니다.");
                    _latestUpgradeIndex = value;
                    PlayerPrefs.SetInt("LatestUpgrade", value);
                }
                else
                {
                    Debug.LogWarning("Upgrade index can not be lower than 50000");
                }
            }
        }

        // 게임 초기화될 때 
        public void Awake()
        {
            DontDestroyOnLoad(this);

            _dataDic = DataDictionary.Instance;

            LoadingFinish = false;

            // Key : Value로써 PlayerPrefs에 저장
            _gold = Convert.ToUInt64(PlayerPrefs.GetString("Gold", "0"));
            _itemcount = PlayerPrefs.GetInt("ItemCount", 0);
            QuestProcess = PlayerPrefs.GetInt("QuestProcess", 90101);
            _leftTimer = new[]
            {
                PlayerPrefs.GetFloat("LeftTimer1", 300.0f), 
                PlayerPrefs.GetFloat("LeftTimer2", 300.0f),
                PlayerPrefs.GetFloat("LeftTimer3", 300.0f)
            };
            _energy = PlayerPrefs.GetInt("Energy", 0);
            _latestUpgradeIndex = PlayerPrefs.GetInt("LatestUpgrade", 50000);

            // 업그레이드 레벨 변수들을 하나로 합침
            UpgradeLv = new UpgradeClass();

            for (var i = 0; i < 12; i++) 
                UpgradeLv[i] = PlayerPrefs.GetInt("Upgrade[" + i + "]LV", 0);

            _newUpgradeInt = PlayerPrefs.GetInt("NewUpgrade", 0);
            TutorialEnd = PlayerPrefs.GetInt("TutorialEnd", 0);

            // 튜토리얼 인덱스를 파트로 구분하여, 파트 중간에 종료 시 파트 처음으로 돌아가게 처리함.
            NowIndex = PlayerPrefs.GetInt("NowIndex", 300101) % 100000 / 100 * 100 + 300001;

            HaveDicPath = "/haveDic.txt";
            ItemOpenListPath = "/itemOpenList.txt";
            NewBookListPath = "/newBookList.txt";
            NewItemListPath = "/newItemList.txt";
            
            HaveDic = LoadGameData(HaveDicPath) as Dictionary<int, Dictionary<int, SerializableVector3>> 
                      ?? new Dictionary<int, Dictionary<int, SerializableVector3>>();
            ItemOpenList = LoadGameData(ItemOpenListPath) as List<int> ?? new List<int>();
            NewBookList = LoadGameData(NewBookListPath) as List<int> ?? new List<int>();
            NewItemList = LoadGameData(NewItemListPath) as List<int> ?? new List<int>();
        }

        private void Start()
        {
            // 종료 후 실행 시간 계산
            for (var i = 0; i < 3; i++) 
                _leftTimer[i] -= TimeAfterLastPlay;

            InvokeRepeating("OnApplicationQuit", 0f, 5f);

            LoadingFinish = true;
        }

        private void Update()
        {
            for (var i = 0; i < 3; i++)
            {
                _leftTimer[i] -= Time.deltaTime;
                PlayerPrefs.SetFloat("LeftTimer" + (i + 1), _leftTimer[i]);
            }
        }

        // 게임 데이터를 불러오는 함수
        private static object LoadGameData(string dataPath)
        {
            var filePath = Application.persistentDataPath + dataPath;

            return File.Exists(filePath) ? DataDeserialize(File.ReadAllBytes(filePath)) : null;
        }

        // 데이터를 역직렬화하는 함수
        private static object DataDeserialize(byte[] buffer)
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();

            mStream.Write(buffer, 0, buffer.Length);
            mStream.Position = 0;

            return binFormatter.Deserialize(mStream);
        }

        // 게임 데이터를 저장하는 함수
        public static void SaveGameData(object data, string dataPath)
        {
            var stream = DataSerialize(data);
            File.WriteAllBytes(Application.persistentDataPath + dataPath, stream);
        }

        // 데이터를 직렬화하는 함수
        private static byte[] DataSerialize(object data)
        {
            var binFormmater = new BinaryFormatter();
            var mStream = new MemoryStream();

            binFormmater.Serialize(mStream, data);

            return mStream.ToArray();
        }

        // 플레이 종료 시간 가져오기
        private static DateTime GetLastPlayDate()
        {
            return !PlayerPrefs.HasKey("Time") ? DateTime.Now : DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("Time")));
        }

        // 어플 종료 시 플레이 시간 저장
        private void OnApplicationQuit()
        {
            PlayerPrefs.SetString("Time", DateTime.Now.ToBinary().ToString());
        }

        // 인벤토리 레벨 업그레이드
        public void UpgradeInvenLv()
        {
            UpgradeLv[0] += 1;
            PlayerPrefs.SetInt("InvenLevel", UpgradeLv[0]);
        }

        /// <summary>
        ///     클릭 당 게이지 레벨 업그레이드
        /// </summary>
        public void UpgradeEnergyPerClickLv()
        {
            UpgradeLv[1] += 1;
        }

        // 업그레이드 인덱스로 최대 업그레이드 레벨 올리기
        public void UnlockUpgrade(int index)
        {
            // 값 길이 = 최대 길이
            UpgradeLv.MaxLv[index - 50001] = _dataDic.FindUpgrade(index).value.Length; 
            NewUpgrade = true;
        }

        public void ResetForDebug()
        {
            PlayerPrefs.DeleteAll();

            HaveDic.Clear();
            ItemOpenList.Clear();
            NewBookList.Clear();
            NewItemList.Clear();

            SaveGameData(HaveDic, HaveDicPath);
            SaveGameData(ItemOpenList, ItemOpenListPath);
            SaveGameData(NewBookList, NewBookListPath);
            SaveGameData(NewItemList, NewItemListPath);

            _gold = Convert.ToUInt64(PlayerPrefs.GetString("Gold", "0"));
            _itemcount = PlayerPrefs.GetInt("ItemCount", 0);
            QuestProcess = PlayerPrefs.GetInt("QuestProcess", 90101);
            
            // 리셋 즉시 시험할 수 있게
            _leftTimer = new[]
            {
                PlayerPrefs.GetFloat("LeftTimer1", 0f), PlayerPrefs.GetFloat("LeftTimer2", 0f),
                PlayerPrefs.GetFloat("LeftTimer3", 0f)
            }; 
            _energy = PlayerPrefs.GetInt("Energy", 0);
            _latestUpgradeIndex = PlayerPrefs.GetInt("LatestUpgrade", 50000);

            UpgradeLv = new UpgradeClass();

            for (var i = 0; i < 12; i++) 
                UpgradeLv[i] = PlayerPrefs.GetInt("Upgrade[" + i + "]LV", 0);
        }

        #region Singleton

        private static DataController _instance;

        public static DataController Instance
        {
            get
            {
                if (_instance != null) 
                    return _instance;
                
                _instance = FindObjectOfType<DataController>();

                if (_instance != null) 
                    return _instance;
                
                var container = new GameObject("DataController");
                
                _instance = container.AddComponent<DataController>();

                return _instance;
            }
        }

        #endregion

        #region Item

        public void InsertNewItem(int key, int itemId)
        {
            InsertNewItem(key, itemId, new Vector3(0, 0, 0));
        }

        /// <summary>
        ///     아이템을 추가하는 함수
        /// </summary>
        /// <param name="key">추가하는 아이템의 Index</param>
        /// <param name="itemId"></param>
        /// <param name="itemPos"></param>
        public void InsertNewItem(int key, int itemId, Vector3 itemPos)
        {
            if (!CheckExistItem(key))
            {
                ItemOpenList.Add(key);
                SaveGameData(ItemOpenList, ItemOpenListPath);

                //Debug.Log("itemOpenList - DataSerialize");

                var posList = new Dictionary<int, SerializableVector3>
                {
                    {itemId, itemPos}
                };

                HaveDic.Add(key, posList);

                //!마크 띄우기
                if (key > 4000) //서적
                {
                    NewBookList.Add(key);
                    SaveGameData(NewBookList, NewBookListPath);
                    var setInfo = _dataDic.CheckSetItemCombine(key);
                    if (setInfo.result != 0) InsertNewItem(setInfo.result, 1);
                }
                else //도감
                {
                    NewItemList.Add(key);
                    SaveGameData(NewItemList, NewItemListPath);
                }
            }
            else
            {
                var id = itemId;
                // id 에러 방지용 땜빵 코드. itemTimer 부분 참조.
                if (HaveDic[key].ContainsKey(id)) 
                    while (HaveDic[key].ContainsKey(id))
                        id++;
                
                HaveDic[key].Add(id, itemPos);
            }

            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObtain(_dataDic.FindItemDic[key]));

            SaveGameData(HaveDic, HaveDicPath);
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
        ///     아이템을 삭제하는 함수
        /// </summary>
        /// <param name="key">삭제할 아이템의 key값</param>
        /// <param name="itemId"></param>
        public void DeleteItem(int key, int itemId)
        {
            if (!HaveDic[key].Remove(itemId)) Debug.Log("DataController - DeleteItem : Item Cannot Delete.");

            SaveGameData(HaveDic, HaveDicPath);
        }

        /// <summary>
        ///     현재 이 아이템을 보유하고 있는지 확인하는 함수
        /// </summary>
        /// <param name="key">haveDic의 key값</param>
        /// <returns></returns>
        private bool CheckExistItem(int key)
        {
            return HaveDic.ContainsKey(key);
        }

        /// <summary>
        ///     현재 보유하고 있는 아이템의 갯수를 보여주는 함수
        /// </summary>
        /// <param name="key">haveDic의 key값</param>
        /// <returns></returns>
        public int GetItemNum(int key)
        {
            return CheckExistItem(key) ? HaveDic[key].Count : 0;
        }

        #endregion

        #region Quest

        public int QuestProcess { get; private set; }

        /// <summary>
        ///     다음 퀘스트로 넘어가기
        /// </summary>
        public void NextQuest()
        {
            QuestProcess += 1;
            PlayerPrefs.SetInt("QuestProcess", QuestProcess);
        }

        #endregion
    }
}