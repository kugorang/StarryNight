#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

#endregion

namespace Script.Common
{
    /// <summary>
    ///     Since unity doesn't flag the Vector3 as serializable, we need to create our own version.
    ///     This one will automatically convert between Vector3 and SerializableVector3.
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

    public class DataController : MonoBehaviour
    {
        private DataDictionary _dataDic;

        // 현재 보유 골드량
        private ulong _gold;

        // 현재 보유 아이템 개수, 퀘스트 진행도(인덱스)
        private int _itemcount, _energy, _latestUpgradeIndex;

        private int _newUpgradeInt;

        // 튜토리얼 현재 인덱스
        private int _nowIndex;

        /// <summary>
        ///     NOTE: 열린 도감을 저장하는 Dictionary
        ///     <para>-> key(int) : 도감이 열린 재료 Index</para>
        /// </summary>
        [HideInInspector] public List<int> ItemOpenList;

        // 신규 항목 알림을 위한 더티 플래그. 서적과 아이템 리스트는 새로운 아이템들의 인덱스를 담음. 길이 0이면 신규 없음.
        // [HideInInspector] public bool newQuest; // 나중에 사용 안 하면 지울 것.

        [HideInInspector] public List<int> NewBookList;
        [HideInInspector] public List<int> NewItemList;

        /// <summary>
        ///     (임시) 이벤트 관찰자 목록. 각관찰자가 등록함.
        /// </summary>
        [HideInInspector] public List<GameObject> Observers;

        /// <summary>
        ///     리셋가능한 리스트
        /// </summary>
        [HideInInspector] public List<GameObject> ResetList;

        // 튜토리얼 완료 여부
        private int TutorialEnd;

        // 아이템 생성 모드 상태
        public static int SwitchButtonMode
        {
            // 0이 별, 1이 나무
            get { return PlayerPrefs.GetInt("SwitchButtonMode", 0); }
            set { PlayerPrefs.SetInt("SwitchButtonMode", value); }
        }

        // HaveDic 정보 저장 경로
        public string HaveDicPath { get; private set; }

        // itemOpenList 정보 저장 경로
        private string ItemOpenListPath { get; set; }

        public bool IsTutorialEnd
        {
            get { return TutorialEnd > 0; }
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
                if (_nowIndex == 300137)
                {
                    _nowIndex = 300201;
                }
                else if (_nowIndex == 300219)
                {
                    _nowIndex = 300301;
                }
                else if (_nowIndex == 300313)
                {
                    _nowIndex = 300401;
                }
                else if (_nowIndex == 300429)
                {
                    // 황소자리 시작
                    _nowIndex = 300501;
                }
                else if (_nowIndex == 300517)
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
                else if (_nowIndex == 300707)
                {
                    _nowIndex = 300801;
                    return 0;
                }
                else if (_nowIndex == 300808)
                {
                    _nowIndex = 300901;
                    return 0;
                }
                else if (_nowIndex == 300906)
                {
                    _nowIndex = 301001;
                    return 0;
                }
                else if (_nowIndex == 301017)
                {
                    _nowIndex = 301101;
                    return 0;
                }
                else if (_nowIndex == 301103)
                {
                    _nowIndex = 301201;
                    return 0;
                }
                else if (_nowIndex == 301205)
                {
                    _nowIndex = 301301;
                    return 0;
                }
                else if (_nowIndex == 301309)
                {
                    _nowIndex = 301401;
                    return 0;
                }
                else if (_nowIndex == 301413)
                {
                    _nowIndex = 301501;
                    return 0;
                }
                else if (_nowIndex == 301539)
                {
                    // 쌍둥이자리 시작
                    _nowIndex = 301601;
                    return 0;
                }
                else if (_nowIndex == 301608)
                {
                    _nowIndex = 301701;
                    return 0;
                }
                else if (_nowIndex == 301729)
                {
                    _nowIndex = 301801;
                    return 0;
                }
                else if (_nowIndex == 301829)
                {
                    _nowIndex = 301901;
                    return 0;
                }
                else if (_nowIndex == 301912)
                {
                    _nowIndex = 302001;
                    return 0;
                }
                else if (_nowIndex == 302004)
                {
                    _nowIndex = 302101;
                    return 0;
                }
                else if (_nowIndex == 302122)
                {
                    _nowIndex = 302201;
                    return 0;
                }
                else if (_nowIndex == 302221)
                {
                    _nowIndex = 302301;
                    return 0;
                }
                else if (_nowIndex == 302323)
                {
                    _nowIndex = 302401;
                    return 0;
                }
                else if (_nowIndex == 302416)
                {
                    _nowIndex = 302501;
                    return 0;
                }
                else if (_nowIndex == 302514)
                {
                    _nowIndex = 302601;
                    return 0;
                }
                else if (_nowIndex == 302609)
                {
                    _nowIndex = 302701;
                    return 0;
                }
                else if (_nowIndex == 302711)
                {
                    _nowIndex = 302801;
                    return 0;
                }
                else if (_nowIndex == 302803)
                {
                    _nowIndex = 302901;
                    return 0;
                }
                else if (_nowIndex == 302909)
                {
                    _nowIndex = 303001;
                    return 0;
                }
                else if (_nowIndex == 303018)
                {
                    _nowIndex = 303101;
                    return 0;
                }
                else if (_nowIndex == 303116)
                {
                    _nowIndex = 303201;
                    return 0;
                }
                else if (_nowIndex == 303228)
                {
                    // 게자리 시작
                    _nowIndex = 303301;
                    return 0;
                }
                else if (_nowIndex == 303350)
                {
                    _nowIndex = 303401;
                    return 0;
                }
                else if (_nowIndex == 303421)
                {
                    _nowIndex = 303501;
                    return 0;
                }
                else if (_nowIndex == 303520)
                {
                    _nowIndex = 303601;
                    return 0;
                }
                else if (_nowIndex == 303631)
                {
                    _nowIndex = 303701;
                    return 0;
                }
                else if (_nowIndex == 303716)
                {
                    _nowIndex = 303801;
                    return 0;
                }
                else if (_nowIndex == 303839)
                {
                    _nowIndex = 303901;
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

        /// <summary>
        ///     새로운 업그레이드가 열렸으면 true
        /// </summary>
        public bool NewUpgrade
        {
            get { return _newUpgradeInt > 0; }
            set
            {
                _newUpgradeInt = value ? 1 : 0;
                PlayerPrefs.SetInt("NewUpgrade", _newUpgradeInt);
            }
        }

        // 게임 초기화될 때 
        public void Awake()
        {
            DontDestroyOnLoad(this);

            _dataDic = DataDictionary.Instance;

            // Key : Value로써 PlayerPrefs에 저장
            _gold = Convert.ToUInt64(PlayerPrefs.GetString("Gold", "0"));
            _itemcount = PlayerPrefs.GetInt("ItemCount", 0);

            _energy = PlayerPrefs.GetInt("Energy", 0);
            _latestUpgradeIndex = PlayerPrefs.GetInt("LatestUpgrade", 50000);

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
            var actualItemCount = 0;

            foreach (var key in HaveDic.Keys)
            {
                // 서적 아이템이 아니면
                if (DataDictionary.IndexToGroup(key) != ItemGroup.Book)
                {
                    actualItemCount += HaveDic[key].Count;
                }   
            }
            
            ItemCount = actualItemCount;
            SceneManager.LoadScene("Logo");
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

        // 인벤토리 레벨 업그레이드
        public void UpgradeInvenLv()
        {
            UpgradeManager.LevelUp(0);
        }

        /// <summary>
        ///     클릭 당 게이지 레벨 업그레이드
        /// </summary>
        public void UpgradeEnergyPerClickLv()
        {
            UpgradeManager.LevelUp(1);
        }

        // 업그레이드 인덱스로 최대 업그레이드 레벨 올리기
        public void UnlockUpgrade(int index)
        {
            // 값 길이 = 최대 길이
            UpgradeManager.MaxLv[index - 50001] = _dataDic.FindUpgrade(index).Value.Length;
            NewUpgrade = true;
        }

        public void ResetData()
        {
            Notify.Text = "";

            if (HaveDic != null)
            {
                HaveDic.Clear();
                SaveGameData(HaveDic, HaveDicPath);
            }

            if (ItemOpenList != null)
            {
                ItemOpenList.Clear();
                SaveGameData(ItemOpenList, ItemOpenListPath);
            }

            if (NewBookList != null)
            {
                NewBookList.Clear();
                SaveGameData(NewBookList, NewBookListPath);
            }

            if (NewItemList != null)
            {
                NewItemList.Clear();
                SaveGameData(NewItemList, NewItemListPath);
            }

            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in ResetList)
            {
                ExecuteEvents.Execute<IResetables>(target, null, (x, y) => x.OnReset());
            }

            NowIndex = 300101;

            PlayerPrefs.DeleteAll();
        }

        // 로딩 상태 가져오기

        #region Properties

        //private const int FirstUpgradeIndex = 50001;
        /// <summary>
        ///     gold 설정
        /// </summary>
        public ulong Gold
        {
            get
            {
                _gold = Convert.ToUInt64(PlayerPrefs.GetString("Gold", "0"));

                return _gold;
            }
            set
            {
                var delta = value - _gold;
                _gold = value;

                PlayerPrefs.SetString("Gold", _gold.ToString());

                // 관찰자들에게 이벤트 메세지 송출
                foreach (var target in Observers)
                {
                    ExecuteEvents.Execute<IEventListener>(target, null,
                        (x, y) => x.OnChangeValue(ValueType.Gold, value, delta));
                }
            }
        }

        /// <summary>
        ///     item 개수
        /// </summary>
        public int ItemCount
        {
            get
            {
                _itemcount = PlayerPrefs.GetInt("ItemCount", 0);

                return _itemcount;
            }
            private set
            {
                _itemcount = value < 0 ? 0 : value;
                PlayerPrefs.SetInt("ItemCount", _itemcount);
            }
        }

        /// <summary>
        ///     item 최대 개수
        /// </summary>
        public int ItemLimit
        {
            get
            {
                var upgradeLv = UpgradeManager.GetUpgradeLv(0);
                return upgradeLv == 0 ? 10 : 10 + _dataDic.FindUpgrade(50001).Value[upgradeLv - 1];
            }
        }

        /// <summary>
        ///     에너지량 가져오기
        /// </summary>
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
                var upgradeLv = UpgradeManager.GetUpgradeLv(1);
                return (upgradeLv == 0 ? 2 : 2 + _dataDic.FindUpgrade(50002).Value[upgradeLv - 1]) * TwiceAll;
            }
        }

        /// <summary>
        ///     쿨타임 감소량 반환.
        /// </summary>
        /// <param name="index">ItemTimer Index(0~2)</param>
        /// <returns></returns>
        public float CoolTimeReduction(int index)
        {
            int[] idList = { 2, 5, 9 };
            var upgradeLv = UpgradeManager.GetUpgradeLv(idList[index]);
            return upgradeLv == 0 ? 0 : _dataDic.FindUpgrade(50001 + idList[index]).Value[upgradeLv - 1];
        }

        /// <summary>
        ///     좋은 아이템 나올 확률
        /// </summary>
        /// <param name="index">ItemTimer Index(0~2)</param>
        /// <returns></returns>
        public int BetterItemProb(int index)
        {
            int[] idList = { 3, 6, 10 };
            var upgradeLv = UpgradeManager.GetUpgradeLv(idList[index]);
            return upgradeLv == 0 ? 0 : _dataDic.FindUpgrade(50001 + idList[index]).Value[upgradeLv - 1];
        }

        /// <summary>
        ///     판매시 추가 골드
        /// </summary>
        public int BonusGold
        {
            get
            {
                var upgradeLv = UpgradeManager.GetUpgradeLv(4);
                return upgradeLv == 0
                    ? 10
                    : 10 + _dataDic.FindUpgrade(50005).Value[upgradeLv - 1];
            }
        }

        /// <summary>
        ///     지구본에서 일반 아이템이 나올 확률(0~95)
        /// </summary>
        public static int AtlasItemProb
        {
            get { return 95 - UpgradeManager.GetUpgradeLv(7); }
        }

        /// <summary>
        ///     조합시 상위 아이템 나올 확률
        /// </summary>
        public int BetterCombineResultProb
        {
            get
            {
                var upgradeLv = UpgradeManager.GetUpgradeLv(8);
                return upgradeLv == 0
                    ? 10
                    : 10 + _dataDic.FindUpgrade(50009).Value[upgradeLv - 1];
            }
        }

        /// <summary>
        ///     마지막 업그레이드가 완료되면 2, 아니면 1.
        /// </summary>
        public static int TwiceAll
        {
            get { return UpgradeManager.GetUpgradeLv(11) > 0 ? 2 : 1; }
        }

        /// <summary>
        ///     열려있는 업그레이드 중 가장 큰 인덱스를 반환합니다. (50001~50012)
        /// </summary>
        public int LatestUpgradeIndex
        {
            get
            {
                _latestUpgradeIndex = PlayerPrefs.GetInt("LatestUpgrade", 50000);

                return _latestUpgradeIndex;
            }
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

        #endregion

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

        /// <summary>
        ///     아이템 인덱스를 받아 그 아이템을 추가하는 함수.
        /// </summary>
        /// <param name="key">아이템 인덱스</param>
        public void InsertNewItem(int key)
        {
            InsertNewItem(key, 1, new Vector3(0, 0, 0));
        }

        public void InsertNewItem(int key, int itemId)
        {
            InsertNewItem(key, itemId, new Vector3(0, 0, 0));
        }

        /// <summary>
        ///     아이템을 추가하는 함수
        /// </summary>
        /// <param name="key">추가하는 아이템의 Index</param>
        /// <param name="itemId">아이템 고유의 id</param>
        /// <param name="itemPos">아이템 위치</param>
        public void InsertNewItem(int key, int itemId, Vector3 itemPos)
        {
            var isBook = DataDictionary.IndexToGroup(key) == ItemGroup.Book;
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

                // !마크 띄우기
                if (isBook) //서적
                {
                    NewBookList.Add(key);
                    SaveGameData(NewBookList, NewBookListPath);
                    var setInfo = _dataDic.CheckSetItemCombine(key);
                    if (setInfo.Result != 0) InsertNewItem(setInfo.Result, 1);
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
                // id 에러 방지용 땜빵 코드. itemid 에 아무 정수나 넣어도 충돌을 방지해준다.
                if (HaveDic[key].ContainsKey(id))
                    while (HaveDic[key].ContainsKey(id))
                        id++;

                HaveDic[key].Add(id, itemPos);
            }

            if (!isBook) //서적이 아니면 아이템 카운트 늘림
                ItemCount += 1;
            // 관찰자들에게 이벤트 메세지 송출
            foreach (var target in Observers)
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObtain(_dataDic.FindItemDic[key]));

            SaveGameData(HaveDic, HaveDicPath);
        }

        /// <summary>
        ///     아이템을 삭제하는 함수
        /// </summary>
        /// <param name="index">삭제할 아이템의 index값</param>
        public void DeleteItem(int index)
        {
            if (!CheckExistItem(index)) //아이템이 없으면 삭제 불가.
            {
                Debug.Log("DataController - DeleteItem : Item Does NOT Exist.");
                return;
            }

            var id = HaveDic[index].Keys.Last(); //마지막 id 제거
            DeleteItem(index, id);
        }


        /// <summary>
        ///     아이템을 삭제하는 함수
        /// </summary>
        /// <param name="index">삭제할 아이템의 index값</param>
        /// <param name="itemId">아이템의 id</param>
        public void DeleteItem(int index, int itemId)
        {
            if (!HaveDic[index].Remove(itemId)) Debug.Log("DataController - DeleteItem : Item Cannot Delete.");
            else if (DataDictionary.IndexToGroup(index) != ItemGroup.Book) //정상적으로 삭제되었고, 서적이 아니면 아이템 카운트 줄임
                ItemCount -= 1;

            SaveGameData(HaveDic, HaveDicPath);
        }

        /// <summary>
        ///     현재 이 아이템을 보유하고 있는지 확인하는 함수
        /// </summary>
        /// <param name="index">haveDic의 index값</param>
        /// <returns></returns>
        private bool CheckExistItem(int index)
        {
            return HaveDic.ContainsKey(index);
        }

        /// <summary>
        ///     현재 보유하고 있는 아이템의 갯수를 보여주는 함수
        /// </summary>
        /// <param name="index">haveDic의 index값</param>
        /// <returns></returns>
        public int GetItemNum(int index)
        {
            return CheckExistItem(index) ? HaveDic[index].Count : 0;
        }

        #endregion
    }
}