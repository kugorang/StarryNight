using UnityEngine;

namespace Script
{
    public class CheckQuestClear : MonoBehaviour
    {
        private GameObject _ariesClear;
        private GameObject _taurusClear;

        private void Awake()
        {
            _ariesClear = GameObject.Find("Aries Clear");
            _taurusClear = GameObject.Find("Taurus Clear");

            _ariesClear.SetActive(false);
            _taurusClear.SetActive(false);

            // 양자리 퀘스트 클리어 시 클리어 이미지 띄우기
            if (DataController.Instance.QuestProcess > 90104) 
                _ariesClear.SetActive(true);

            // 황소자리 퀘스트 클리어 시 클리어 이미지 띄우기
            if (DataController.Instance.QuestProcess > 90123) 
                _taurusClear.SetActive(true);
        }
    }
}