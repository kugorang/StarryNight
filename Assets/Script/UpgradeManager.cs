using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script
{

   
    public class UpgradeManager : MonoBehaviour, IResetables
    {

        #region Upgrade Variables
        
        public static int[] MaxLv = new int[12];

        private static int[] _upgradeLv=new int[12];//Readonly로 만들지 말 것.
       
        public void OnReset()
        {
            _upgradeLv = new int[12];
        }

        /// <summary>
        /// id를 통해 현재 업그레이드 레벨을 반환합니다.
        /// </summary>
        /// <param name="index">양자리(0)~물고기자리(11)</param>
        /// <returns></returns>
        public static int GetUpgradeLV(int index)
        {
            if (-1 < index && index < 12)
            {
                _upgradeLv[index] = PlayerPrefs.GetInt("Upgrade[" + index + "]LV", 0);
                return _upgradeLv[index];
            }


            if (50000 < index && index <= 50012)
            {
                _upgradeLv[index - 50001] = PlayerPrefs.GetInt("Upgrade[" + (index - 50001) + "]LV", 0);
                return _upgradeLv[index - 50001];
            }

        Debug.LogError("Out of Index; Index should between 0~11 or 50001~50012");
            return 0;
        }

        /// <summary>
        /// id에 해당하는 별자리 레벨을 올림.
        /// </summary>
        /// <param name="id">양자리(0)~물고기자리(11)</param>
        public static void LevelUp(int id)
        {
            _upgradeLv[id]++;
            PlayerPrefs.SetInt("Upgrade[" + id + "]LV", _upgradeLv[id]);
        }
        #endregion

        private const int MaxLevel = 20;
        private const int LastUpgradeMaxLevel = 1;
        private DataController _dataController;
        private DataDictionary _dataDic;

        // 업그레이드 잠금 표시
        private Transform[] _parentPanels;

        public int[] SuccessRate;
        private readonly GameObject[] _unlockPanels = new GameObject[12];
        private readonly Button[] _upgradeButtons = new Button[12];
        private readonly Text[] _upgradeCostDisplayers = new Text[12];
        private readonly Text[] _upgradeDisplayers = new Text[12];

        private void Awake()
        {
            _dataDic = GameObject.FindWithTag("DataController").GetComponent<DataDictionary>();

            _parentPanels = new[]
            {
                GameObject.Find("Upgrade Panel (0)").transform,
                GameObject.Find("Upgrade Panel (1)").transform,
                GameObject.Find("Upgrade Panel (2)").transform,
                GameObject.Find("Upgrade Panel (3)").transform,
                GameObject.Find("Upgrade Panel (4)").transform,
                GameObject.Find("Upgrade Panel (5)").transform,
                GameObject.Find("Upgrade Panel (6)").transform,
                GameObject.Find("Upgrade Panel (7)").transform,
                GameObject.Find("Upgrade Panel (8)").transform,
                GameObject.Find("Upgrade Panel (9)").transform,
                GameObject.Find("Upgrade Panel (10)").transform,
                GameObject.Find("Upgrade Panel (11)").transform
            };

            for (var i = 0; i < 12; i++)
            {
                _unlockPanels[i] = _parentPanels[i].Find("Unlock Panel").gameObject;
                _upgradeDisplayers[i] = _parentPanels[i].Find("Upgrade Displayer").gameObject.GetComponent<Text>();
                _upgradeButtons[i] = _parentPanels[i].Find("Upgrade Button").gameObject.GetComponent<Button>();
                _upgradeCostDisplayers[i] = _upgradeButtons[i].transform.Find("Upgrade Cost Displayer").gameObject
                    .GetComponent<Text>();
            }

            _dataController = DataController.Instance;

            if (SuccessRate.Length < 12) 
                SuccessRate = new int[12];
        }

        private void Update()
        {
            for (var i = 0; i < 12; i++)
            {
                var upgradeIndex = 50001 + i;
                
                // 업그레이드 text 및 버튼 설정
                if (upgradeIndex > _dataController.LatestUpgradeIndex)
                {
                    _upgradeButtons[i].enabled = false;
                    _unlockPanels[i].SetActive(!IsMaxUpgraded(upgradeIndex));
                }
                else
                {
                    _upgradeButtons[i].enabled = true;
                    _unlockPanels[i].SetActive(false);
                }

                // value[current값] -> 꼭!
                if (IsMaxUpgraded(upgradeIndex))
                {
                    _upgradeDisplayers[i].text = "MAX";
                    _upgradeCostDisplayers[i].text = "MAX";
                }
                else
                {
                    var nextUpgradeValue = _dataDic.FindUpDic[upgradeIndex].Value[GetUpgradeLV(i)];
                    string str;
                    
                    switch (i) 
                    {
                        case 0:
                            str = "인벤토리 +" + _dataController.ItemLimit + " -> +" + (10 + nextUpgradeValue);
                            break;
                        case 1:
                            str = "클릭당 게이지 +" + _dataController.EnergyPerClick + " -> +" + (2 + nextUpgradeValue);
                            break;
                        case 2:
                            str = "아이템 타이머1 시간 감소 " + _dataController.CoolTimeReduction(0) + "초 -> " + nextUpgradeValue + "초";
                            break;
                        case 3:
                            str = "아이템 타이머1에서 좋은 아이템 나올 확률 증가 +" + _dataController.BetterItemProb(0) + "% -> +" + nextUpgradeValue +
                                  "%";
                            break;
                        case 4:
                            str = "판매시 골드 추가 지급 +" + _dataController.BonusGold + "골드 -> +" + nextUpgradeValue + "골드";
                            break;
                        case 5:
                            str = "아이템 타이머2 시간 감소 " + _dataController.CoolTimeReduction(1) + "초 -> " + nextUpgradeValue + "초";
                            break;
                        case 6:
                            str = "아이템 타이머2에서 좋은 아이템 나올 확률 증가 +" + _dataController.BetterItemProb(1) + "% -> +" + nextUpgradeValue +
                                  "%";
                            break;
                        case 7:
                            str = "지구본에서 한 단계 상위 아이템 나올 확률 증가" + _dataController.AtlasItemProb + "% -> +" + nextUpgradeValue +
                                  "%";
                            break;
                        case 8:
                            str = "아이템 조합 시 한 단계 상위 아이템 나올 확률 증가 +" + _dataController.BetterCombineResultProb + "% -> +" +
                                  nextUpgradeValue + "%";
                            break;
                        case 9:
                            str = "아이템 타이머3 시간 감소 " + _dataController.CoolTimeReduction(2) + "초 -> " + nextUpgradeValue + "초";
                            break;
                        case 10:
                            str = "아이템 타이머3에서 좋은 아이템 나올 확률 증가 +" + _dataController.BetterItemProb(2) + "% -> +" + nextUpgradeValue +
                                  "%";
                            break;
                        // 최종 업그레이드
                        default: 
                            str = "골드 2배, 인벤토리 2배, 높은 등급 아이템 나올 확률 2배, 아이템 2개 나올 확률 2배";
                            break;
                    }

                    _upgradeDisplayers[i].text = str;
                    _upgradeCostDisplayers[i].text = _dataDic.FindUpgrade(upgradeIndex).Cost[GetUpgradeLV(i)] + "골드";
                }
            }
        }

        // 아래 두 함수 하나로, 실패 구현
        public void Upgrade(int upgradeIndex)
        {
            var id = upgradeIndex - 50001;
            
            if (IsMaxUpgraded(upgradeIndex))
            {
                Debug.Log("Level Max");
                return;
            }

            // Debug.Log("Upgrade index: " + upgradeIndex);
            var prob = SuccessRate[GetUpgradeLV(id)] / 100f;

            if (_dataController.Gold < (ulong) _dataDic.FindUpgrade(upgradeIndex).Cost[GetUpgradeLV(id)])
            {
                PopUpWindow.Alert("골드가 부족해요.");
                return;
            }

            Random.InitState((int) Time.time);
            PopUpWindow.SetSliderValue(prob);
            _dataController.Gold -= (ulong) _dataDic.FindUpgrade(upgradeIndex).Cost[GetUpgradeLV(id)];
            
            // 업그레이드 레벨은 0 ~ 20이고 20에선 업글 불가
            if (Random.value < prob) 
            {
                Action onComplete = () => PopUpWindow.Alert("업그레이드 성공!");
                
                PopUpWindow.AnimateSlider(1, 0.6f, onComplete);
                LevelUp(id);
                
                // 관찰자들에게 이벤트 메세지 송출
                foreach (var target in _dataController.Observers) 
                    ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this, id));
            }
            else
            {
                Action onComplete = () => PopUpWindow.Alert("업그레이드 실패...");
              
                
                PopUpWindow.AnimateSlider(0, 0.6f, onComplete);
            }
        }

        //TODO: 테스트 중. "다음 업그레이드"가 0인 업그레이드를 선택했을 때 작동해야함.
        public void RemoveAlert() 
        {
            _dataController.NewUpgrade = false;
        }

        private static bool IsMaxUpgraded(int upgradeIndex)
        {
            if (upgradeIndex == 50012)
                return GetUpgradeLV(upgradeIndex) >= LastUpgradeMaxLevel;
            
            return GetUpgradeLV(upgradeIndex) >= MaxLevel;
        }
    }
}