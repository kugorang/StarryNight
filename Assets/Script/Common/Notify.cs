#region

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Script.Common
{
    public class Notify : MonoBehaviour, IResetables
    {
        private const int ReferenceW = 1080;
        private static Text _notifyText;
        private static RectTransform _rtf;
        private static bool _isMinimized; // default value : false
        private static bool _enabled = true;
        private static GameObject _gameObj;
        private static Image _btnImage;
        private static DataController _dataController;

        private static string _notification = "";
        private static string _formerString = ""; // ShowQuestProgress를 했을 때만 _formerString이 ""이 아님.

        /// <summary>
        ///     알림창에 띄울 내용을 할당합니다. 빈 문자열을 할당하면, 알림창이 자동으로 비활성화 됩니다.
        /// </summary>
        public static string Text
        {
            get { return _notification; }
            set
            {
                _notification = value;

                // 빈 문자열이 아니면 활성화, 빈 문자열이면 비활성화
                _enabled = _notification != "";

                if (_notifyText == null)
                    return;

                _notifyText.text = _notification;

                if (_isMinimized)
                    _gameObj.GetComponent<Notify>().Toggle();
                else // Toggle엔 Refresh가 포함되어 있다.
                    Refresh();
            }
        }

        public void OnReset()
        {
            Text = "";
            Refresh();
        }

        // Use this for initialization
        // Notify는 Dialog에 있어 매 씬 이동마다 파괴후 재 생성된다.
        private void Awake()
        {
            Initialize();

            /*// 씬 전환 전에도 빈 문자열이면 숨긴다.
            if (_notification != "") 
                return;*/

            Text = "";
        }

        private void Start()
        {
            _dataController = DataController.Instance;

            if (!_dataController.ResetList.Contains(gameObject))
                _dataController.ResetList.Add(gameObject);
        }

        private static void Initialize()
        {
            _gameObj = GameObject.Find("Notification Displayer");

            if (_gameObj == null)
            {
                Debug.LogWarning("Notification object is destroyed and not able to notify.");
                return;
            }

            _notifyText = _gameObj.GetComponentInChildren<Text>();
            _rtf = _gameObj.GetComponent<RectTransform>();
            _btnImage = _gameObj.GetComponent<Image>();
        }

        /// <summary>
        ///     창의 크기와 위치를 알맞게 변경.
        /// </summary>
        private static void Refresh()
        {
            _btnImage.raycastTarget = _enabled;
            _btnImage.color = _enabled ? new Color(0f, 0f, 0f, 0.5f) : new Color(0.5f, 0.5f, 0.5f, 0);
            _notifyText.color = _enabled ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 0f, 0f, 0f);

            // TODO: 해상도에 따라 다르게 지원할 것.
            if (_isMinimized)
            {
                _rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                _rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
                // 현재 Canvas Scaler설정 탓에 Screen.Width를 쓸 필요가 없음.
                _rtf.anchoredPosition3D = new Vector3(ReferenceW / 2 - 60, -220, 0);
                _notifyText.text = "?";
            }
            else
            {
                // 한 줄 당 약 15글자, 줄 하나당 추가로 높이 약 50 필요. 3줄 이상(띄어쓰기 포함 45 자 이상)은 필요 없을 거라 추정함.
                var height = _notification.Length / 15 * 50 + 100;
                _rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ReferenceW);
                _rtf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                _rtf.anchoredPosition3D =
                    new Vector3(0, -160 - height / 2, 0); // -160은 Displayer 패널의 끝, 따라서 중심은 그보다 height/2 만큼.
                _notifyText.text = _notification;
            }
        }

        /// <summary>
        ///     최소화 상태와 최대화 상태에 따라 크기 전환.
        ///     버튼으로 호출할 수 있어야해서 static이 아님
        /// </summary>
        public void Toggle()
        {
            _isMinimized = !_isMinimized;
            Refresh();
        }

        // 임시 코드. ItemLimit Image에 버튼과 Raycast Target을 둠. 이 기능을 삭제하면 이것도 원상복구할 것
        public void ShowQuestProgress()
        {
            if (_formerString == "")
            {
                _formerString = Text;
                var questIndex = Quest.Progress;
                var dataDic = DataDictionary.Instance;
                var ownQuest = dataDic.FindQuestDic[questIndex];
                var dataController = DataController.Instance;

                // 퀘스트 진행상태 출력
                switch (questIndex)
                {
                    case 90101: // 별의 원석 아무거나 1개 획득
                    {
                        int[] itemIndex = {1001, 1006, 1011};
                        var questItemNum = itemIndex.Sum(i => dataController.GetItemNum(i));

                        Text = string.Format("별의 원석 {0} / 1", questItemNum);
                        break;
                    }
                    case 90102: // 재료 아이템 아무거나 1개 획득
                    {
                        int[] itemIndex = {2001, 2006, 2011, 2016, 2021, 2026};
                        var questItemNum = itemIndex.Sum(i => dataController.GetItemNum(i));

                        Text = string.Format("재료 아이템 {0} / 1", questItemNum);
                        break;
                    }
                    case 90103: // 별의 파편 아무거나 1개 획득
                    {
                        int[] itemIndex = {1002, 1007, 1012};
                        var questItemNum = itemIndex.Sum(i => dataController.GetItemNum(i));

                        Text = string.Format("별의 파편 {0} / 1", questItemNum);
                        break;
                    }
                    case 90202: // 세트 아이템 재료 1개 획득 성공
                    {
                        int[] itemIndex =
                        {
                            4001, 4002, 4003, 4004, 4006, 4007, 4008, 4009,
                            4011, 4012, 4013, 4014, 4016, 4017, 4018, 4019,
                            4021, 4022, 4023, 4024, 4026, 4027, 4028, 4029,
                            4031, 4032, 4033, 4034, 4036, 4037, 4038, 4039,
                            4041, 4042, 4043, 4044, 4046, 4047, 4048, 4049,
                            4051, 4052, 4053, 4054, 4056, 4057, 4058
                        };

                        var questItemNum = itemIndex.Sum(i => dataController.GetItemNum(i));
                        Text = string.Format("세트 아이템 재료 {0} / 1", questItemNum);
                        break;
                    }
                    default:
                        var text = "";
                        var isTermFirst = true;

                        foreach (var term in ownQuest.TermsDic)
                        {
                            var termItemIndex = term.Key;

                            if (termItemIndex == 9999) // 조건이 골드일 때
                                text += isTermFirst
                                    ? string.Format("골드 {0} / {1}", dataController.Gold, term.Value)
                                    : string.Format("\n골드 {0} / {1}", dataController.Gold, term.Value);
                            else if (termItemIndex > 50000) // 조건이 업그레이드일 때
                                text += isTermFirst
                                    ? string.Format("{0} {1} / {2}", dataDic.FindUpgrade(termItemIndex).Name,
                                        UpgradeManager.GetUpgradeLv(termItemIndex), term.Value)
                                    : string.Format("\n{0} {1} / {2}", dataDic.FindUpgrade(termItemIndex).Name,
                                        UpgradeManager.GetUpgradeLv(termItemIndex), term.Value);
                            else
                                text += isTermFirst
                                    ? string.Format("{0} {1} / {2}", dataDic.FindItemDic[termItemIndex].Name,
                                        dataController.GetItemNum(termItemIndex), term.Value)
                                    : string.Format("\n{0} {1} / {2}", dataDic.FindItemDic[termItemIndex].Name,
                                        dataController.GetItemNum(termItemIndex), term.Value);

                            isTermFirst = false;
                        }

                        Text = text;
                        break;
                }
            }
            else
            {
                Text = _formerString;
                _formerString = "";
            }
        }

        // Awake 이후 재 검증
        private void OnEnable()
        {
            if (_gameObj == null)
                Initialize();
        }

        private void OnDisable()
        {
            _dataController.ResetList.Remove(gameObject);

            _gameObj = null;
            _rtf = null;
            _notifyText = null;
            _btnImage = null;
        }
    }
}