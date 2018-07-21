using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script
{
    public class CreateItem : MonoBehaviour
    {
        // Item ID 공유 변수
        private static int _idCount;

        private static CreateItem _instance;

        private DataController _dataController;
        private DataDictionary _dataDic;
        
        public Button Btn;
        private int _energyMaxValue;        // 에너지 충전 최대량
        private int _energyPerClick;        // 클릭당 에너지 증가량
        public Image ImgEarthback;          // 게이지 이미지
        public GameObject Item;             // 아이템

        public static CreateItem Instance
        {
            get
            {
                if (_instance != null) 
                    return _instance;
                
                _instance = FindObjectOfType<CreateItem>();

                if (_instance != null) 
                    return _instance;
                
                var container = new GameObject("CreateItem");
                _instance = container.AddComponent<CreateItem>();

                return _instance;
            }
        }

        private void Awake()
        {
            _dataController = DataController.Instance;

            _dataDic = DataDictionary.Instance;
            /*_energyPerClick = DataController.Instance.EnergyPerClick;*/
            // TODO: 개발 시 시간 절약을 위해 100으로 설정. 개발이 끝나고 출시하면 지울 것!
            _energyPerClick = 100; 
            _energyMaxValue = 100;

            _idCount = PlayerPrefs.GetInt("IdCount", 0);
        }

        private void Start()
        {
            if (ImgEarthback == null) 
                ImgEarthback = GameObject.Find("EarthBack").GetComponent<Image>();

            if (Btn == null) 
                Btn = gameObject.GetComponent<Button>();

            if (_dataController.HaveDic != null)
                foreach (var entry in _dataController.HaveDic)
                {
                    // 서적이면 만들지 않는다.
                    if (entry.Key > 4000) 
                        continue;
                    
                    // do something with entry. Value or entry.Key                
                    foreach (var secondEntry in entry.Value)
                        GenerateItem(entry.Key, false, secondEntry.Key, secondEntry.Value);
                }

            ImgEarthback.fillAmount = (float) _dataController.Energy / _energyMaxValue;
        }

        // 게이지 클릭
        public void OnClick()
        {
            // 아이템 갯수 제한
            if (_dataController.ItemCount >= _dataController.ItemLimit) 
            {
                PopUpWindow.Alert("창고가 꽉 차있어요!");
                return;
            }
            
            var energy = _dataController.Energy + _energyPerClick;
            ImgEarthback.fillAmount = (float)energy / _energyMaxValue;
            _dataController.Energy = energy;
            
            // 현재 에너지가 필요한 에너지보다 작은 경우
            if (_dataController.Energy < _energyMaxValue)
                return;

            // 나무일 때
            if (SwitchMode.Instance.State)
            {
                GenerateItem(Random.Range(0, 100) >= DataController.AtlasItemProb
                    ? Random.Range(0, 6) * 5 + 2002 : Random.Range(0, 6) * 5 + 2001, true);
            }
            else // 별일 때
            {
                // 5%의 확률로 별의 잔재 등장
                if (Random.Range(0, 100) < 5)
                    GenerateItem(5001, true);
                else
                    GenerateItem(Random.Range(0, 100) >= DataController.AtlasItemProb
                        ? Random.Range(0, 3) * 5 + 1002 : Random.Range(0, 3) * 5 + 1001, true);
            }

            Btn.enabled = false;
            StartCoroutine(DecreaseEnergy());
            
            AudioManager.Instance.ClickSound();
            AudioManager.Instance.ItemSound();
            
            // 관찰자들에게 Click 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));
        }
        
        // 에너지 감소
        private IEnumerator DecreaseEnergy()
        {
            while (Math.Abs(ImgEarthback.fillAmount) > 0)
            {
                yield return new WaitForSeconds(0.05f);
                ImgEarthback.fillAmount -= 0.1f;
            }

            _dataController.Energy = 0;
            Btn.enabled = true;

            yield return null;
        }

        // C# 에서는 디폴트 파라미터를 허용하지 않기 때문에 이렇게 함수 오버로딩을 통해 구현하였습니다.
        private void GenerateItem(int productId, bool isNew)
        {
            GenerateItem(productId, isNew, -1, new Vector3(-758, -284, -3));
        }

        private void GenerateItem(int productId, bool isNew, int itemId, Vector3 itemPos)
        {
           
            var newItem = Instantiate(Item, itemPos, Quaternion.identity);

            var findItemInfo = _dataDic.FindItemDic[productId];
            var itemInstance = newItem.GetComponent<Item>();
            itemInstance.SetItemInfo(productId, findItemInfo);

            newItem.GetComponent<BoxCollider2D>().isTrigger = false;
            newItem.GetComponent<SpriteRenderer>().sprite =
                Resources.Load<Sprite>(_dataDic.FindItemDic[productId].ImagePath);

            // 새로 만들어진 아이템이라면 애니메이션을 실행한다.
            if (isNew)
            {
                itemInstance.Id = _idCount++;
                PlayerPrefs.SetInt("IdCount", _idCount);

                itemInstance.StartAnimation();
                _dataController.InsertNewItem(productId, itemInstance.Id, itemInstance.Pos);
            }
            else
            {

                itemInstance.Id = itemId;
                DataController.SaveGameData(_dataController.HaveDic, _dataController.HaveDicPath);
            }
        }
    }
}